using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        Task<UpdateUserRequest> GetUserDetails(int userId);
        Task<int> UpdateUser(UpdateUserRequest request);
        Task<List<UserModel>> GetAllUser();
        Task<int> SetUserStatus(UserStatusRequest request);
    }
}
