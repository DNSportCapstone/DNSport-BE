using BusinessObject.Models;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class StadiumService: IStadium
    {
        private Db12353Context _dbcontext = new();

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
