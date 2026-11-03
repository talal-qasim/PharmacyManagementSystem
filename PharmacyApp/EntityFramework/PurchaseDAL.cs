using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class PurchaseDAL
    {
        public static async Task<List<Purchase>> GetPurchases()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Purchases
                        .Include(x => x.Supplier)
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.PurchaseDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Purchase>();
            }
        }

        public static async Task<Purchase> GetPurchaseById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Purchases
                        .Include(x => x.Supplier)
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<Purchase> Insert(Purchase entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    entity.Status = PurchaseStatusEnum.Draft;
                    dataContext.Purchases.Add(entity);
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
        /// Only allowed while the purchase is still Draft (Section 23) — header
        /// fields (Supplier, date, reference) can be corrected before receiving.
        /// </summary>
        public static async Task<Purchase> Update(Purchase entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Purchases.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found == null || found.Status != PurchaseStatusEnum.Draft)
                    {
                        return null;
                    }

                    found.SupplierId = entity.SupplierId;
                    found.PurchaseDate = entity.PurchaseDate;
                    found.ReferenceNumber = entity.ReferenceNumber;

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

        /// <summary>
        /// Only Draft purchases can be soft-deleted (Section 23/35) — a Received
        /// purchase has already created real inventory and must remain historical.
        /// </summary>
        public static async Task<bool> Delete(Purchase entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Purchases.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null && found.Status == PurchaseStatusEnum.Draft)
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