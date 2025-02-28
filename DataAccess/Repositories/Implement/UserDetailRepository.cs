using AutoMapper;
using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implement
{
    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly UserDetailDAO _userDetailDAO;
        private readonly IMapper _mapper;
        public UserDetailRepository(UserDetailDAO userDetailDAO, IMapper mapper)
        {
            _userDetailDAO = userDetailDAO;
            _mapper = mapper;
        }
        public async Task AddNewUserDetail(UserDetailModel userDetailModel)
        {
            try
            {
                var userDetail = _mapper.Map<UserDetail>(userDetailModel);
                if (userDetail != null)
                {
                    await _userDetailDAO.InsertUserDetail(userDetail);
                }
            }
            catch
            {
            }
        }
    }
}
