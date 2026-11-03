using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// Plain CRUD for Batch metadata (batch number, expiry, cost, received date).
    /// Deliberately has NO Insert or a quantity-changing Update path — Quantity only
    /// ever changes through InventoryDAL, which keeps StockTransaction rows in sync
    /// (Section 51). Use InventoryDAL.ReceiveBatch(...) to create a new batch.
    /// </summary>
    public class BatchDAL
    {
        public static async Task<List<Batch>> GetBatches()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Batches
                        .Include(x => x.ProductVariant)
                            .ThenInclude(v => v.Medicine)
                        .Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Batch>();
            }
        }

        public static async Task<Batch> GetBatchById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Batches
                        .Include(x => x.ProductVariant)
                            .ThenInclude(v => v.Medicine)
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        /// <summary>
        /// Updates only metadata fields (BatchNumber, ExpiryDate, PurchaseCost,
        /// ReceivedDate). Quantity changes on the passed entity are intentionally
        /// NOT persisted here — call InventoryDAL.AdjustStock for that.
        /// </summary>
        public static async Task<Batch> Update(Batch entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Batches.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found == null) return null;

                    found.BatchNumber = entity.BatchNumber;
                    found.ExpiryDate = entity.ExpiryDate;
                    found.PurchaseCost = entity.PurchaseCost;
                    found.ReceivedDate = entity.ReceivedDate;

                    await dataContext.SaveChangesAsync();
                    return found;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<bool> Delete(Batch entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Batches.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null)
                    {
                        found.IsDeleted = true;
                        await dataContext.SaveChangesAsync();
                        return true;
                    }
                    return false;
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