using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> AddNewUser(UserModel userModel);
        Task<UserModel> GetUserByEmail(string email);
        Task<UpdateUserRequest> GetUserDetails(int userId);
        Task<int> UpdateUser(UpdateUserRequest request);
        Task<UserModel> GetUserById(int userId);
    }
}
