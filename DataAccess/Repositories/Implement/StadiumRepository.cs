using BusinessObject.Models;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class StadiumRepository : IStadiumRepository
    {
        private readonly Db12353Context _dbcontext;
        public StadiumRepository(Db12353Context dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<StadiumModel>> GetStadiumData()
        {
            var result = await (from s in _dbcontext.Stadium
                                select new StadiumModel
                                {
                                    StadiumId = s.StadiumId,
                                    UserId = s.UserId,
                                    StadiumName = s.StadiumName,
                                    Address = s.Address,
                                    Image = s.Image,
                                    Status = s.Status
                                }).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Stadium> AddStadium(StadiumRequestModel model)
        {
            var stadium = new Stadium
            {
                UserId = model.UserId,
                StadiumName = model.StadiumName,
                Address = model.Address,
                Image = model.Image,
                Status = model.Status
            };
            await _dbcontext.Stadium.AddAsync(stadium);
            await _dbcontext.SaveChangesAsync();
            return stadium;
        }
    }
}
