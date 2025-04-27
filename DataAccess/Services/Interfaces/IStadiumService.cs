
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IStadiumService
    {
        Task<int> DisableStadium(int id, string status);
        Task<List<StadiumModel>> GetStadiumsByUserId(int userId);
        Task<List<StadiumResponse>> GetPendingStadiumsAsync();
        Task<bool> UpdateStadiumStatusAsync(UpdateStadiumStatusRequest request);

    }
}