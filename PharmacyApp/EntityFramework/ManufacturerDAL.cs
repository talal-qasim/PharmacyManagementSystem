using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class ManufacturerDAL
    {
        public static async Task<List<Manufacturer>> GetManufacturers()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Manufacturers
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Manufacturer>();
            }
        }

        public static async Task<Manufacturer> GetManufacturerById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Manufacturers
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<Manufacturer> Insert(Manufacturer entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Manufacturers.Add(entity);
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

        public static async Task<Manufacturer> Update(Manufacturer entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Manufacturers.Update(entity);
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

        public static async Task<bool> Delete(Manufacturer entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Manufacturers
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
