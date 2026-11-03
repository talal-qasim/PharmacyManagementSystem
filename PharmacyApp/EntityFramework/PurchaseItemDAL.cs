using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class PurchaseItemDAL
    {
        public static async Task<List<PurchaseItem>> GetPurchaseItemsByPurchaseId(int purchaseId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.PurchaseItems
                        .Include(x => x.ProductVariant)
                            .ThenInclude(v => v.Medicine)
                        .Where(x => x.PurchaseId == purchaseId)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<PurchaseItem>();
            }
        }

        public static async Task<PurchaseItem> Insert(PurchaseItem entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    // Guard: only allow adding items while the parent Purchase is Draft (Section 23).
                    var purchase = await dataContext.Purchases.FirstOrDefaultAsync(x => x.Id == entity.PurchaseId);
                    if (purchase == null || purchase.Status != PurchaseStatusEnum.Draft)
                    {
                        return null;
                    }

                    dataContext.PurchaseItems.Add(entity);
                    await dataContext.SaveChangesAsync();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<PurchaseItem> Update(PurchaseItem entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var purchase = await dataContext.Purchases.FirstOrDefaultAsync(x => x.Id == entity.PurchaseId);
                    if (purchase == null || purchase.Status != PurchaseStatusEnum.Draft)
                    {
                        return null;
                    }

                    dataContext.PurchaseItems.Update(entity);
                    await dataContext.SaveChangesAsync();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        /// <summary>
        /// Hard delete is acceptable here (unlike master data) because a PurchaseItem
        /// on a Draft purchase has no historical significance yet — it hasn't created
        /// a Batch or StockTransaction. Once Received, items are locked (see guard above).
        /// </summary>
        public static async Task<bool> Delete(PurchaseItem entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var purchase = await dataContext.Purchases.FirstOrDefaultAsync(x => x.Id == entity.PurchaseId);
                    if (purchase == null || purchase.Status != PurchaseStatusEnum.Draft)
                    {
                        return false;
                    }

                    var found = await dataContext.PurchaseItems.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null)
                    {
                        dataContext.PurchaseItems.Remove(found);
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