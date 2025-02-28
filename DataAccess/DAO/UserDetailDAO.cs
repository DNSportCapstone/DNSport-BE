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
        public UserDetail GetUserDetailById(int id)
        {
            using var context = new Db12353Context();
            return context.UserDetails.FirstOrDefault(c => c.UserId == id) ?? new UserDetail();
        }

        public async Task InsertUserDetail(UserDetail userDetail)
        {
            using var context = new Db12353Context();
            context.UserDetails.Add(userDetail);
            await context.SaveChangesAsync();
        }

        public void UpdateUserDetail(UserDetail userDetail)
        {
            using var context = new Db12353Context();
            context.UserDetails.Update(userDetail);
            context.SaveChanges();
        }

        public void DeleteUserDetail(UserDetail userDetail)
        {
            using var context = new Db12353Context();
            context.UserDetails.Remove(userDetail);
            context.SaveChanges();
        }
    }
}
