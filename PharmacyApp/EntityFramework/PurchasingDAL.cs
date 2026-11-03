using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// The one operation that turns a Draft Purchase into real inventory
    /// (Sections 21-23, 27, 51 — "Purchase receiving" is listed as an atomic
    /// business operation touching Purchase + PurchaseItems + Batch + Stock +
    /// StockTransaction together).
    /// </summary>
    public class PurchasingDAL
    {
        public static async Task<bool> ReceivePurchase(int purchaseId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var purchase = await dataContext.Purchases
                        .FirstOrDefaultAsync(x => x.Id == purchaseId && x.IsDeleted == false);

                    if (purchase == null || purchase.Status != PurchaseStatusEnum.Draft)
                    {
                        return false; // already received, cancelled, or doesn't exist
                    }

                    var items = await dataContext.PurchaseItems
                        .Where(x => x.PurchaseId == purchaseId)
                        .ToListAsync();

                    if (items.Count == 0)
                    {
                        return false; // nothing to receive
                    }

                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        foreach (var item in items)
                        {
                            var batch = new Batch
                            {
                                ProductVariantId = item.ProductVariantId,
                                BatchNumber = item.BatchNumber,
                                ExpiryDate = item.ExpiryDate,
                                ReceivedDate = DateTime.Now,
                                PurchaseCost = item.PurchaseCost,
                                Quantity = item.Quantity
                            };

                            dataContext.Batches.Add(batch);
                            await dataContext.SaveChangesAsync(); // assigns batch.Id

                            dataContext.StockTransactions.Add(new StockTransaction
                            {
                                ProductVariantId = item.ProductVariantId,
                                BatchId = batch.Id,
                                QuantityChange = item.Quantity,
                                TransactionType = StockTransactionTypeEnum.Purchase,
                                TransactionDate = DateTime.Now,
                                Reference = $"Purchase #{purchase.Id}",
                                Reason = "Purchase received"
                            });
                        }

                        purchase.Status = PurchaseStatusEnum.Received;
                        purchase.ReceivedDate = DateTime.Now;

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