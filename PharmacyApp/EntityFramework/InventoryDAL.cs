using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// The single place that ever changes a Batch's Quantity. Every method here
    /// opens one DataContext, wraps the batch update + StockTransaction insert in
    /// a single EF Core database transaction (Section 51 — atomic operations), and
    /// commits or rolls back together. This is where FEFO (Section 16) lives.
    /// </summary>
    public class InventoryDAL
    {
        /// <summary>
        /// Section 21-23 — creates a new Batch and its matching Purchase-type
        /// StockTransaction atomically. Used by BatchViewModel for manual entry now,
        /// and will be reused by the Purchase-receiving workflow in Step 5.
        /// </summary>
        public static async Task<Batch> ReceiveBatch(Batch batch, string reference = null, int? userId = null)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        userId = userId ?? GlobalEntities.CurrentUser?.Id;
                        dataContext.Batches.Add(batch);
                        await dataContext.SaveChangesAsync(); // assigns batch.Id

                        dataContext.StockTransactions.Add(new StockTransaction
                        {
                            ProductVariantId = batch.ProductVariantId,
                            BatchId = batch.Id,
                            QuantityChange = batch.Quantity,
                            TransactionType = StockTransactionTypeEnum.Purchase,
                            TransactionDate = DateTime.Now,
                            Reference = reference,
                            Reason = "Batch received",
                            UserId = userId
                        });

                        await dataContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();
                        return batch;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        /// <summary>
        /// Section 26 — check before attempting a sale/deduction.
        /// Only counts non-expired, non-deleted batches with quantity > 0.
        /// </summary>
        public static async Task<int> GetAvailableStock(int productVariantId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Batches
                        .Where(x => x.ProductVariantId == productVariantId
                                 && x.IsDeleted == false
                                 && x.Quantity > 0
                                 && x.ExpiryDate >= DateTime.Now)
                        .SumAsync(x => (int?)x.Quantity) ?? 0;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        /// <summary>
        /// THE FEFO ALGORITHM (Section 16, 27). Deducts requestedQuantity from a
        /// ProductVariant's batches, soonest-expiry-first (oldest-received as
        /// tiebreaker), possibly splitting across multiple batches in one call.
        /// Rejects the ENTIRE operation — no partial deduction — if total available
        /// stock is insufficient (Section 26 — never allow negative stock).
        /// Returns true/false rather than throwing, so callers (e.g. Sales in Step 6)
        /// can show a clean "insufficient stock" message instead of a crash.
        /// </summary>
        public static async Task<bool> DeductStockFefo(
            int productVariantId,
            int requestedQuantity,
            StockTransactionTypeEnum transactionType,
            string reference,
            string reason,
            int? userId = null)
        {
            try
            {
                if (requestedQuantity <= 0)
                {
                    return false;
                }

                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        userId = userId ?? GlobalEntities.CurrentUser?.Id;
                        var batches = await dataContext.Batches
                            .Where(x => x.ProductVariantId == productVariantId
                                     && x.IsDeleted == false
                                     && x.Quantity > 0
                                     && x.ExpiryDate >= DateTime.Now)
                            .OrderBy(x => x.ExpiryDate)      // FEFO — soonest expiry first
                            .ThenBy(x => x.ReceivedDate)     // tiebreaker — oldest received first
                            .ToListAsync();

                        int totalAvailable = batches.Sum(x => x.Quantity);
                        if (totalAvailable < requestedQuantity)
                        {
                            // Section 26 — reject the whole sale, do not partially deduct.
                            return false;
                        }

                        int remaining = requestedQuantity;
                        foreach (var batch in batches)
                        {
                            if (remaining <= 0) break;

                            int deductFromThisBatch = Math.Min(batch.Quantity, remaining);
                            batch.Quantity -= deductFromThisBatch;
                            remaining -= deductFromThisBatch;

                            dataContext.StockTransactions.Add(new StockTransaction
                            {
                                ProductVariantId = productVariantId,
                                BatchId = batch.Id,
                                QuantityChange = -deductFromThisBatch,
                                TransactionType = transactionType,
                                TransactionDate = DateTime.Now,
                                Reference = reference,
                                Reason = reason,
                                UserId = userId
                            });
                        }

                        await dataContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return false;
            }
        }

        /// <summary>
        /// Sections 29-30 — manual stock adjustment / damage / expiry removal on ONE
        /// specific batch (unlike DeductStockFefo, which auto-picks batches). Positive
        /// quantityChange adds stock back (e.g. reversing a sale, Section 32); negative
        /// reduces it (damage, physical-count shortfall). Rejects if it would go negative.
        /// </summary>
        public static async Task<bool> AdjustStock(
            int batchId,
            int quantityChange,
            StockTransactionTypeEnum transactionType,
            string reason,
            int? userId = null)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        userId = userId ?? GlobalEntities.CurrentUser?.Id;
                        var batch = await dataContext.Batches
                            .FirstOrDefaultAsync(x => x.Id == batchId && x.IsDeleted == false);

                        if (batch == null)
                        {
                            return false;
                        }

                        int newQuantity = batch.Quantity + quantityChange;
                        if (newQuantity < 0)
                        {
                            return false; // Section 26 — no negative stock
                        }

                        batch.Quantity = newQuantity;

                        dataContext.StockTransactions.Add(new StockTransaction
                        {
                            ProductVariantId = batch.ProductVariantId,
                            BatchId = batch.Id,
                            QuantityChange = quantityChange,
                            TransactionType = transactionType,
                            TransactionDate = DateTime.Now,
                            Reason = reason,
                            UserId = userId
                        });

                        await dataContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return false;
            }
        }
        public static async Task<bool> RestoreStockOnCancellation(
            int productVariantId,
            int quantity,
            string reference,
            string reason,
            int? userId = null)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        userId = userId ?? GlobalEntities.CurrentUser?.Id;
                        // Find the oldest/soonest-expiring active batch
                        var batch = await dataContext.Batches
                            .Where(x => x.ProductVariantId == productVariantId && !x.IsDeleted)
                            .OrderBy(x => x.ExpiryDate)
                            .FirstOrDefaultAsync();

                        if (batch == null)
                        {
                            // If no batch exists at all, we can't restore stock without creating a dummy batch,
                            // which is complex. For MVP, just fail safely or log it.
                            return false;
                        }

                        batch.Quantity += quantity;

                        dataContext.StockTransactions.Add(new StockTransaction
                        {
                            ProductVariantId = productVariantId,
                            BatchId = batch.Id,
                            QuantityChange = quantity,
                            TransactionType = StockTransactionTypeEnum.SaleCancellation,
                            TransactionDate = DateTime.Now,
                            Reference = reference,
                            Reason = reason,
                            UserId = userId
                        });

                        await dataContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return false;
            }
        }
    }
}