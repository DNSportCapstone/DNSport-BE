using BusinessObject.Models;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IUserDetailRepository
    {
        Task AddNewUserDetail(UserDetailModel userDetailModel);
    }
}
