using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class SalesTransactionDAL
    {
        public static async Task<bool> FinalizeSale(int saleId, PaymentMethodEnum paymentMethod, decimal amountPaid)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        var sale = await dataContext.Sales
                            .Include(x => x.SaleItems)
                            .FirstOrDefaultAsync(x => x.Id == saleId);

                        if (sale == null || sale.Status == SaleStatusEnum.Completed)
                            return false;

                        // Process inventory for each item
                        foreach (var item in sale.SaleItems)
                        {
                            bool deducted = await InventoryDAL.DeductStockFefo(
                                item.ProductVariantId,
                                item.Quantity,
                                StockTransactionTypeEnum.Sale,
                                $"Sale #{saleId}",
                                "Sale finalized"
                            );

                            if (!deducted)
                            {
                                // Insufficient stock, abort transaction
                                await dbTransaction.RollbackAsync();
                                return false;
                            }
                        }

                        // Create payment
                        dataContext.Payments.Add(new Payment
                        {
                            SaleId = saleId,
                            Method = paymentMethod,
                            AmountPaid = amountPaid,
                            PaymentDate = DateTime.Now
                        });

                        sale.Status = SaleStatusEnum.Completed;
                        
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

        public static async Task<bool> CancelSale(int saleId, string reason)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    using (var dbTransaction = await dataContext.Database.BeginTransactionAsync())
                    {
                        var sale = await dataContext.Sales
                            .Include(x => x.SaleItems)
                            .FirstOrDefaultAsync(x => x.Id == saleId);

                        if (sale == null || sale.Status != SaleStatusEnum.Completed)
                            return false;

                        foreach (var item in sale.SaleItems)
                        {
                            bool restored = await InventoryDAL.RestoreStockOnCancellation(
                                item.ProductVariantId,
                                item.Quantity,
                                $"Sale #{saleId} Cancelled",
                                reason
                            );

                            if (!restored)
                            {
                                await dbTransaction.RollbackAsync();
                                return false;
                            }
                        }

                        sale.Status = SaleStatusEnum.Cancelled;
                        
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
