using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IStadiumRepository
    {
        Task<List<StadiumModel>> GetStadiumData();
        Task<Stadium> AddStadium(StadiumRequestModel model);
        Task<bool> UpdateStadiumImage(int id, string imageUrl);
    }
}
