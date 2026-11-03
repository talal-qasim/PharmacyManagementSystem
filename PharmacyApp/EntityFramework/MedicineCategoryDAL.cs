using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// Static DAL for MedicineCategory (per rulebook Rule 7.3).
    /// Every method opens its own fresh DataContext inside a using block,
    /// is async, wraps logic in try-catch, logs via Common.HandleException,
    /// and returns a safe fallback value on failure. Deletes are soft (IsDeleted = true).
    /// </summary>
    public class MedicineCategoryDAL
    {
        public static async Task<List<MedicineCategory>> GetMedicineCategories()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.MedicineCategories
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<MedicineCategory>();
            }
        }

        public static async Task<MedicineCategory> GetMedicineCategoryById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.MedicineCategories
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<MedicineCategory> Insert(MedicineCategory entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.MedicineCategories.Add(entity);
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

        public static async Task<MedicineCategory> Update(MedicineCategory entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.MedicineCategories.Update(entity);
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

        public static async Task<bool> Delete(MedicineCategory entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.MedicineCategories
                        .FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null)
                    {
                        found.IsDeleted = true; // Soft delete
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