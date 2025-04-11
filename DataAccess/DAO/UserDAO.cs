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
        private readonly Db12353Context _dbcontext;
        public UserDAO(Db12353Context dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _dbcontext.Users.Include(u => u.UserDetail).ToListAsync();
        }

        public User GetUserById(int id)
        {
            return _dbcontext.Users.FirstOrDefault(c => c.UserId == id) ?? new User();
        }
        public User GetUserInfomationById(int id)
        {
            return _dbcontext.Users.Include(u => u.UserDetail ?? null)
                                .Include(u => u.BankingAccounts)
                                .Include(u => u.Role)
                                .AsNoTracking()
                                .FirstOrDefault(c => c.UserId == id) ?? new User();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbcontext.Users.Include(u => u.UserDetail).FirstOrDefaultAsync(c => c.Email == email) ?? new User();
        }
        public async Task<bool> IsExistUser(string email)
        {
            return await _dbcontext.Users.AnyAsync(c => c.Email == email);
        }

        public async Task InsertUser(User user)
        {
            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();
        }

        public void UpdateUser(User user)
        {
            _dbcontext.Users.Update(user);
            _dbcontext.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _dbcontext.Users.Remove(user);
            _dbcontext.SaveChanges();
        }
    }
}
