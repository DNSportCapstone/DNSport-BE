using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IStadiumRepository
    {
        Task<List<StadiumModel>> GetStadiumData();
        Task<List<StadiumModel>> GetStadiumByName(string stadiumName);
        Task<Stadium> AddStadium(StadiumRequestModel model);
        Task<int> DisableStadium(int id, string status);
        Task<IEnumerable<StadiumLessorModel>> GetStadiumsByLessorIdAsync(int userId);
        Task<List<StadiumModel>> GetStadiumsByUserId(int userId);
        Task<List<StadiumModel>> GetPendingStadiums();
        Task<bool> UpdateStadiumStatus(int stadiumId, string newStatus);
    }
}