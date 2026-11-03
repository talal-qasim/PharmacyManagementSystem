using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    public class SupplierDAL
    {
        public static async Task<List<Supplier>> GetSuppliers()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Suppliers
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<Supplier>();
            }
        }

        public static async Task<Supplier> GetSupplierById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Suppliers
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<Supplier> Insert(Supplier entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Suppliers.Add(entity);
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

        public static async Task<Supplier> Update(Supplier entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Suppliers.Update(entity);
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

        public static async Task<bool> Delete(Supplier entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Suppliers
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
