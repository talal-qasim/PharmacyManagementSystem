using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class ReportDAL
    {
        public static async Task<List<Sale>> GetSalesReport(DateTime from, DateTime to)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Sales
                        .Include(x => x.SaleItems)
                            .ThenInclude(si => si.ProductVariant)
                                .ThenInclude(pv => pv.Medicine)
                        .Where(x => !x.IsDeleted && x.SaleDate.Date >= from.Date && x.SaleDate.Date <= to.Date)
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

        public static async Task<List<Sale>> GetDailySalesReport(DateTime date)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Sales
                        .Include(x => x.SaleItems)
                            .ThenInclude(si => si.ProductVariant)
                                .ThenInclude(pv => pv.Medicine)
                        .Where(x => !x.IsDeleted && x.SaleDate.Date == date.Date)
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

        public static async Task<List<Purchase>> GetPurchaseReport(DateTime from, DateTime to)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Purchases
                        .Include(x => x.Supplier)
                        .Where(x => !x.IsDeleted && x.PurchaseDate.Date >= from.Date && x.PurchaseDate.Date <= to.Date)
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

        public static async Task<List<ProductVariant>> GetLowStockReport()
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

                    var allVariants = await dataContext.ProductVariants
                        .Include(x => x.Medicine)
                        .Where(x => !x.IsDeleted)
                        .ToListAsync();

                    var lowStockList = new List<ProductVariant>();

                    foreach(var v in allVariants)
                    {
                        var stock = variantStock.FirstOrDefault(x => x.ProductVariantId == v.Id);
                        if (stock == null || stock.TotalQuantity <= v.MinimumStockLevel)
                        {
                            lowStockList.Add(v);
                        }
                    }

                    return lowStockList;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<ProductVariant>();
            }
        }

        public static async Task<List<Batch>> GetExpiryReport()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var allBatches = await dataContext.Batches
                        .Include(x => x.ProductVariant)
                            .ThenInclude(pv => pv.Medicine)
                        .Where(x => !x.IsDeleted && x.Quantity > 0)
                        .ToListAsync();

                    return allBatches
                        .Where(x => x.ExpiryStatus != ExpiryStatusEnum.Normal)
                        .OrderBy(x => x.ExpiryDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Batch>();
            }
        }

        public static async Task<List<StockTransaction>> GetStockMovementReport(int productVariantId, DateTime from, DateTime to)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.StockTransactions
                        .Include(x => x.ProductVariant)
                            .ThenInclude(pv => pv.Medicine)
                        .Include(x => x.Batch)
                        .Where(x => x.ProductVariantId == productVariantId 
                                 && x.TransactionDate.Date >= from.Date 
                                 && x.TransactionDate.Date <= to.Date)
                        .OrderByDescending(x => x.TransactionDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<StockTransaction>();
            }
        }
    }
}
