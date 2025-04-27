using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class StadiumService : IStadiumService
    {
        private readonly IStadiumRepository _stadiumRepository;
        public StadiumService(IStadiumRepository stadiumRepository)
        {
            _stadiumRepository = stadiumRepository;
        }
        public async Task<int> DisableStadium(int id, string status)
        {
            return await _stadiumRepository.DisableStadium(id, status);
        }
        public async Task<List<StadiumModel>> GetStadiumsByUserId(int userId)
        {
            return await _stadiumRepository.GetStadiumsByUserId(userId);
        }
        public async Task<List<StadiumResponse>> GetPendingStadiumsAsync()
        {
            var stadiums = await _stadiumRepository.GetPendingStadiums();

            return stadiums.Select(s => new StadiumResponse
            {
                StadiumId = s.StadiumId,
                StadiumName = s.StadiumName,
                Address = s.Address,
                Status = s.Status,
                Image = s.Image
            }).ToList();
        }
        public async Task<bool> UpdateStadiumStatusAsync(UpdateStadiumStatusRequest request)
        {
            return await _stadiumRepository.UpdateStadiumStatus(request.StadiumId, request.NewStatus);
        }
    }

}
