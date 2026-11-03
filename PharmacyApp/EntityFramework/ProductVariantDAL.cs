using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class ProductVariantDAL
    {
        /// <summary>
        /// Variants belong to exactly one Medicine — this is the query used by
        /// ProductVariantListViewModel when "Manage Variants" is opened for a medicine.
        /// </summary>
        public static async Task<List<ProductVariant>> GetProductVariantsByMedicineId(int medicineId)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.ProductVariants
                        .Where(x => x.MedicineId == medicineId && x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<ProductVariant>();
            }
        }
        /// <summary>
        /// Flat list of every variant across every medicine, with Medicine included —
        /// used by BatchView's "select a variant" combo box (Section 21 — a batch is
        /// received against one specific sellable variant).
        /// </summary>
        public static async Task<List<ProductVariant>> GetAllProductVariants()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.ProductVariants
                        .Include(x => x.Medicine)
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<ProductVariant>();
            }
        }

        public static async Task<ProductVariant> GetProductVariantById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.ProductVariants
                        .Include(x => x.Medicine)
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<ProductVariant> Insert(ProductVariant entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.ProductVariants.Add(entity);
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

        public static async Task<ProductVariant> Update(ProductVariant entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.ProductVariants.Update(entity);
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

        public static async Task<bool> Delete(ProductVariant entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.ProductVariants.FirstOrDefaultAsync(x => x.Id == entity.Id);
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