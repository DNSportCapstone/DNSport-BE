using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IFieldRepository _fieldRepository;
        public UserService(IUserRepository userRepository, IEmailSender emailSender, IFieldRepository fieldRepository)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _fieldRepository = fieldRepository;
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

        public async Task<int> SetUserStatus(UserStatusRequest request)
        {
            return await _userRepository.SetUserStatus(request);
        }

        public async Task<int> SetUserRole(int userId, int roleId)
        {
            return await _userRepository.SetUserRole(userId, roleId);
        }

        public async Task<bool> WarningLessor(WarningRequest request)
        {
            try
            {
                var fieldByBooking = await _fieldRepository.GetFieldsByBookingId(request.BookingId);
                var field = await _fieldRepository.GetFieldByIdAsync(fieldByBooking.FieldId);
                var user = await _fieldRepository.GetFieldOwner(field.FieldId);
                await _emailSender.SendEmailAsync(user.Email, $"Cảnh báo về tình trạng sân: {field.Description}, {field.Stadium.StadiumName}", request.Message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
