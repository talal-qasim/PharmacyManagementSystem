using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class SaleDAL
    {
        public static async Task<List<Sale>> GetSales()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Sales
                        .Where(x => !x.IsDeleted)
                        .OrderByDescending(x => x.SaleDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Sale>();
            }
        }

        public static async Task<Sale> GetSaleById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Sales
                        .Include(x => x.SaleItems)
                            .ThenInclude(si => si.ProductVariant)
                                .ThenInclude(pv => pv.Medicine)
                        .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<Sale> Insert(Sale entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Sales.Add(entity);
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
    }
}
