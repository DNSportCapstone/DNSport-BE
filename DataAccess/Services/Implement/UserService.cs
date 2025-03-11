using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services.Implement
{
    public class UserService : IUserService
    {
        private Db12353Context _dbcontext = new();

        // To Do
        public async Task<UpdateUserRequest> GetUserDetails(int userId)
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
                                select new UpdateUserRequest
                                {
                                    UserId = u.UserId,
                                    RoleId = u.RoleId,
                                    Email = u.Email,
                                    Status = u.Status,
                                    Account = ba.Account,
                                    Bank = ba.Bank,
                                    PhoneNumber = ud.PhoneNumber,
                                    FullName = ud.FullName,
                                    Address = ud.Address,
                                    RoleName = r.RoleName,
                                    ReceiveNotification = u.ReceiveNotification
                                })
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> UpdateUser(UpdateUserRequest request)
        {
            var u = await _dbcontext.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            if (u is not null)
            {
                u.ReceiveNotification = request.ReceiveNotification;
            }
            //update user detail
            var user = await _dbcontext.UserDetails.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            if (user is not null)
            {
                user.PhoneNumber = request.PhoneNumber;
                user.FullName = request.FullName;
                user.Address = request.Address;
                await _dbcontext.SaveChangesAsync();
            }
            return user.UserId;
        }
    }
}
