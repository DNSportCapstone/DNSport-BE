using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserDetailDAO
    {
        private readonly Db12353Context _dbcontext;
        public UserDetailDAO(Db12353Context dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public UserDetail GetUserDetailById(int id)
        {
            return _dbcontext.UserDetails.FirstOrDefault(c => c.UserId == id) ?? new UserDetail();
        }

        public async Task InsertUserDetail(UserDetail userDetail)
        {
            _dbcontext.UserDetails.Add(userDetail);
            await _dbcontext.SaveChangesAsync();
        }

        public void UpdateUserDetail(UserDetail userDetail)
        {
            _dbcontext.UserDetails.Update(userDetail);
            _dbcontext.SaveChanges();
        }

        public void DeleteUserDetail(UserDetail userDetail)
        {
            _dbcontext.UserDetails.Remove(userDetail);
            _dbcontext.SaveChanges();
        }
    }
}
