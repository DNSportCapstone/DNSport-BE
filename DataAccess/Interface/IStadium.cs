using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Interface
{
    public interface IStadium
    {
        Task<List<StadiumModel>> GetStadiumData();
        Task<Stadium> AddStadium(StadiumRequestModel model);
    }
}
