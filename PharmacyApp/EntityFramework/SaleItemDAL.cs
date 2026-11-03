using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class SaleItemDAL
    {
        public static async Task<List<SaleItem>> GetSaleItemsBySaleId(int saleId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.SaleItems
                        .Include(x => x.ProductVariant)
                            .ThenInclude(v => v.Medicine)
                        .Where(x => x.SaleId == saleId)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<SaleItem>();
            }
        }

        public static async Task<SaleItem> Insert(SaleItem entity)
        {
            try
            {
                int availableStock = await InventoryDAL.GetAvailableStock(entity.ProductVariantId);
                if (availableStock < entity.Quantity)
                {
                    return null; // Reject, insufficient stock
                }

                using (DataContext dataContext = new DataContext())
                {
                    dataContext.SaleItems.Add(entity);
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

        public static async Task<bool> Delete(SaleItem entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.SaleItems.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null)
                    {
                        dataContext.SaleItems.Remove(found);
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
