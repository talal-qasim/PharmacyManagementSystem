using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class DashboardDAL
    {
        public static async Task<int> GetTotalActiveProducts()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.ProductVariants
                        .Include(pv => pv.Medicine)
                        .Where(x => !x.IsDeleted && x.Medicine.Status == MedicineStatusEnum.Active)
                        .CountAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<int> GetOutOfStockCount()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    // An aggregate query to find variants whose total non-expired stock is 0
                    var variantStock = await dataContext.Batches
                        .Where(b => !b.IsDeleted && b.ExpiryDate >= DateTime.Now)
                        .GroupBy(b => b.ProductVariantId)
                        .Select(g => new { ProductVariantId = g.Key, TotalQuantity = g.Sum(b => b.Quantity) })
                        .ToListAsync();
                        
                    var allVariants = await dataContext.ProductVariants.Where(x => !x.IsDeleted).Select(x => x.Id).ToListAsync();
                    
                    // Out of stock = variants with 0 in the grouping + variants with no batches at all
                    int outOfStock = 0;
                    foreach(var v in allVariants)
                    {
                        var stock = variantStock.FirstOrDefault(x => x.ProductVariantId == v);
                        if (stock == null || stock.TotalQuantity == 0)
                        {
                            outOfStock++;
                        }
                    }
                    return outOfStock;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<int> GetLowStockCount()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var variantStock = await dataContext.Batches
                        .Where(b => !b.IsDeleted && b.ExpiryDate >= DateTime.Now)
                        .GroupBy(b => b.ProductVariantId)
                        .Select(g => new { ProductVariantId = g.Key, TotalQuantity = g.Sum(b => b.Quantity) })
                        .ToListAsync();

                    var variants = await dataContext.ProductVariants.Where(x => !x.IsDeleted).ToListAsync();
                    
                    int lowStock = 0;
                    foreach(var v in variants)
                    {
                        var stock = variantStock.FirstOrDefault(x => x.ProductVariantId == v.Id);
                        if (stock != null && stock.TotalQuantity > 0 && stock.TotalQuantity <= v.MinimumStockLevel)
                        {
                            lowStock++;
                        }
                    }
                    return lowStock;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<int> GetNearExpiryBatchCount()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var batches = await dataContext.Batches
                        .Where(x => !x.IsDeleted && x.Quantity > 0)
                        .ToListAsync();
                    return batches.Count(x => x.ExpiryStatus == ExpiryStatusEnum.NearExpiry);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<int> GetExpiredBatchCount()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var batches = await dataContext.Batches
                        .Where(x => !x.IsDeleted && x.Quantity > 0)
                        .ToListAsync();
                    return batches.Count(x => x.ExpiryStatus == ExpiryStatusEnum.Expired);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<int> GetTodaySalesCount()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Sales
                        .Where(x => x.SaleDate.Date == DateTime.Now.Date && x.Status == SaleStatusEnum.Completed && !x.IsDeleted)
                        .CountAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<decimal> GetTodayRevenue()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var todaySales = await dataContext.Sales
                        .Include(x => x.SaleItems)
                        .Where(x => x.SaleDate.Date == DateTime.Now.Date && x.Status == SaleStatusEnum.Completed && !x.IsDeleted)
                        .ToListAsync();
                        
                    return todaySales.Sum(x => x.TotalAmount);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<decimal> GetMonthlySalesTotal()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var thisMonthSales = await dataContext.Sales
                        .Include(x => x.SaleItems)
                        .Where(x => x.SaleDate.Year == DateTime.Now.Year && x.SaleDate.Month == DateTime.Now.Month && x.Status == SaleStatusEnum.Completed && !x.IsDeleted)
                        .ToListAsync();
                        
                    return thisMonthSales.Sum(x => x.TotalAmount);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return 0;
            }
        }

        public static async Task<List<Purchase>> GetRecentPurchases(int count = 5)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Purchases
                        .Include(x => x.Supplier)
                        .Where(x => !x.IsDeleted)
                        .OrderByDescending(x => x.PurchaseDate)
                        .Take(count)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Purchase>();
            }
        }
    }
}
