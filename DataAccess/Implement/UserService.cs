using BusinessObject.Models;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class UserService : IUser
    {
        private Db12353Context _dbcontext = new();
        public async Task<UserModel> GetUserDetails(int userId)
        {
            var result = await (from u in _dbcontext.Users
                                join d in _dbcontext.UserDetails
                                on u.UserId equals d.UserId into userDetail
                                from ud in userDetail.DefaultIfEmpty()
                                join b in _dbcontext.BankingAccounts
                                on u.UserId equals b.UserId into bankingAccount
                                from ba in bankingAccount.DefaultIfEmpty()
                                join r in _dbcontext.Roles
                                on u.RoleId equals r.RoleId
                                where u.UserId == userId
                                select new UserModel
                                {
                                    UserId = u.UserId,
                                    RoleId = u.RoleId,
                                    Email = u.Email,
                                    Status = u.Status,
                                    PhoneNumber = ud.PhoneNumber,
                                    FullName = ud.FullName,
                                    Account = ba.Account,
                                    Bank = ba.Bank,
                                    RoleName = r.RoleName,
                                    Address = ud.Address
                                })
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> UpdateUser(UserModel model)
        {
            //update user
            var user = await _dbcontext.UserDetails.FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if(user is not null)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;
                user.Address = model.Address;
                await _dbcontext.SaveChangesAsync();
            }
            return user.UserId;
        }
    }
}
