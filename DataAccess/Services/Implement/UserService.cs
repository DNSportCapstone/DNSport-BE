using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // To Do
        public async Task<UpdateUserRequest> GetUserDetails(int userId)
        {
            return await _userRepository.GetUserDetails(userId);
        }

        public async Task<int> UpdateUser(UpdateUserRequest request)
        {
            return await _userRepository.UpdateUser(request);
        }

        public async Task<List<UserModel>> GetAllUser()
        {
            return await _userRepository.GetAllUser();
        }
    }
}
