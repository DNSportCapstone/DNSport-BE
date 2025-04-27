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

    }

}
