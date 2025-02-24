﻿using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> AddNewUser(UserModel userModel);
        Task<UserModel> GetUserByEmail(string email);
    }
}
