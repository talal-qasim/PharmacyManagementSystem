using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// Read-only DAL for StockTransaction. No Insert/Update/Delete exposed here on
    /// purpose (Section 28) — rows are only ever created as a side effect of
    /// InventoryDAL's atomic operations, never directly from a generic CRUD form.
    /// </summary>
    public class StockTransactionDAL
    {
        public static async Task<List<StockTransaction>> GetStockTransactions()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.StockTransactions
                        .Include(x => x.ProductVariant)
                            .ThenInclude(v => v.Medicine)
                        .Include(x => x.Batch)
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

        public static async Task<List<StockTransaction>> GetStockTransactionsByProductVariantId(int productVariantId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.StockTransactions
                        .Include(x => x.Batch)
                        .Where(x => x.ProductVariantId == productVariantId)
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