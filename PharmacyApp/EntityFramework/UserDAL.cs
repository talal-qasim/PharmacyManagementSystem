using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;
using Common;

namespace PharmacyApp.EntityFramework
{
    public class UserDAL
    {
        public static async Task<List<User>> GetUsers()
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Users
                        .Where(x => !x.IsDeleted)
                        .OrderBy(x => x.FullName)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return new List<User>();
            }
        }

        public static async Task<User> GetUserById(int id)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    return await dataContext.Users
                        .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<User> Authenticate(string username, string password)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var user = await dataContext.Users
                        .FirstOrDefaultAsync(x => x.Username == username && !x.IsDeleted && x.IsActive);
                    
                    if (user != null)
                    {
                        if (PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                        {
                            return user;
                        }
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<User> Insert(User entity, string plainPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(plainPassword))
                {
                    PasswordHasher.HashPassword(plainPassword, out string hash, out string salt);
                    entity.PasswordHash = hash;
                    entity.PasswordSalt = salt;
                }

                using (DataContext dataContext = new DataContext())
                {
                    dataContext.Users.Add(entity);
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

        public static async Task<User> Update(User entity, string newPlainPassword = null)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    if (found != null)
                    {
                        found.FullName = entity.FullName;
                        found.Username = entity.Username;
                        found.Role = entity.Role;
                        found.IsActive = entity.IsActive;

                        if (!string.IsNullOrWhiteSpace(newPlainPassword))
                        {
                            PasswordHasher.HashPassword(newPlainPassword, out string hash, out string salt);
                            found.PasswordHash = hash;
                            found.PasswordSalt = salt;
                        }

                        await dataContext.SaveChangesAsync();
                        return found;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Common.HandleException.FoundException(ex);
                return null;
            }
        }

        public static async Task<bool> Delete(User entity)
        {
            try
            {
                using (DataContext dataContext = new DataContext())
                {
                    var found = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == entity.Id);
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
