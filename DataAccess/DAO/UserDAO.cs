using BusinessObject.Models;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserDAO
    {
        public async Task<List<User>> GetAllUsers()
        {
            using var context = new Db12353Context();
            return await context.Users.Include(u => u.UserDetail).ToListAsync();
        }

        public User GetUserById(int id)
        {
            using var context = new Db12353Context();
            return context.Users.FirstOrDefault(c => c.UserId == id) ?? new User();
        }
        public User GetUserInfomationById(int id)
        {
            using var context = new Db12353Context();
            return context.Users.Include(u => u.UserDetail ?? null)
                                .Include(u => u.BankingAccounts)
                                .Include(u => u.Role)
                                .AsNoTracking()
                                .FirstOrDefault(c => c.UserId == id) ?? new User();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using var context = new Db12353Context();
            return await context.Users.Include(u => u.UserDetail).FirstOrDefaultAsync(c => c.Email == email) ?? new User();
        }
        public async Task<bool> IsExistUser(string email)
        {
            using var context = new Db12353Context();
            return await context.Users.AnyAsync(c => c.Email == email);
        }

        public async Task InsertUser(User user)
        {
            using var context = new Db12353Context();
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        public void UpdateUser(User user)
        {
            using var context = new Db12353Context();
            context.Users.Update(user);
            context.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            using var context = new Db12353Context();
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }
}
