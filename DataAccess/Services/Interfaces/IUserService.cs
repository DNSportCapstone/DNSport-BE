﻿using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUserDetails(int userId);
        Task<int> UpdateUser(UpdateUserRequest request);
    }
}
