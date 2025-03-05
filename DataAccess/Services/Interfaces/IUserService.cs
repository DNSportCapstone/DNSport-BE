using DataAccess.DTOs.Request;

namespace DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        Task<UpdateUserRequest> GetUserDetails(int userId);
        Task<int> UpdateUser(UpdateUserRequest request);
    }
}
