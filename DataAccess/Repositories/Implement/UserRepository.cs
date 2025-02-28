using BusinessObject.Models;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.DAO;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;
        private readonly IMapper _mapper;
        public UserRepository(UserDAO userDAO, IMapper mapper)
        {
            _userDAO = userDAO;
            _mapper = mapper;
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
    }
}
