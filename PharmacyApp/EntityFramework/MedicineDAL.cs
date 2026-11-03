using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class MedicineDAL
    {
        public static async Task<List<Medicine>> GetMedicines()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Medicines
                        .Include(x => x.MedicineCategory)
                        .Include(x => x.Manufacturer)
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Medicine>();
            }
        }

        public static async Task<Medicine> GetMedicineById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Medicines
                        .Include(x => x.MedicineCategory)
                        .Include(x => x.Manufacturer)
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<Medicine> Insert(Medicine entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Medicines.Add(entity);
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

        public static async Task<Medicine> Update(Medicine entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Medicines.Update(entity);
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

        public static async Task<bool> Delete(Medicine entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Medicines.FirstOrDefaultAsync(x => x.Id == entity.Id);
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