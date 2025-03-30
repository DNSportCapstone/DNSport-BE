using BusinessObject.Models;
using DataAccess.Model;
using AutoMapper;
using DataAccess.DAO;
using DataAccess.Repositories.Interfaces;
using DataAccess.DTOs.Request;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;
        private readonly IMapper _mapper;
        private Db12353Context _dbcontext;
        public UserRepository(UserDAO userDAO, IMapper mapper, Db12353Context dnContext)
        {
            _userDAO = userDAO;
            _mapper = mapper;
            _dbcontext = dnContext;
        }

        public async Task<UserModel> AddNewUser(UserModel userModel)
        {
            try
            {
                var user = _mapper.Map<User>(userModel);

                if (user != null)
                {
                    await _userDAO.InsertUser(user);
                }

                return _mapper.Map<UserModel>(user);
            }
            catch
            {
                return new UserModel();
            }
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userDAO.GetUserByEmail(email);
                return _mapper.Map<UserModel>(user);
            }
            catch
            {
                return new UserModel();
            }
        }

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

        public async Task<UserModel> GetUserById(int userId)
        {
            try
            {
                var user = await _userDAO.GetUserById(userId);
                return _mapper.Map<UserModel>(user);
            }
            catch
            {
                return new UserModel();
            }
        }
        
        public async Task<List<UserModel>> GetAllUser()
        {
            var users = await _userDAO.GetAllUsers();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task<UserModel> GetUserByBookingId(int bookingId)
        {
            var booking = await _dbcontext.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingId);
            var user = await _dbcontext.Users
                                       .Include(u => u.UserDetail)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(c => c.UserId == booking.UserId);
           
            return _mapper.Map<UserModel>(user);
        }
    }
}
