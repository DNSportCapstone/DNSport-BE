using DataAccess.Model;

namespace DataAccess.Interface
{
    public interface IUser
    {
        Task<UserModel> GetUserDetails(int userId);
        Task<int> UpdateUser(UserModel model);
    }
}
