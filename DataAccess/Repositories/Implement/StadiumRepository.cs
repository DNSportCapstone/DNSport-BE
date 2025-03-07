using BusinessObject.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implement
{
    public class StadiumRepository : IStadiumRepository
    {
        Db12353Context _dbcontext = new Db12353Context();
        private readonly Cloudinary _cloudinary;

        public StadiumRepository(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<StadiumModel>> GetStadiumData()
        {
            var result = await (from s in _dbcontext.Stadiums
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
            await _dbcontext.Stadiums.AddAsync(stadium);
            await _dbcontext.SaveChangesAsync();
            return stadium;
        }


        public async Task<bool> UpdateStadiumImage(int id, string imageUrl)
        {
            var stadium = await _dbcontext.Stadiums.FindAsync(id);
            if (stadium == null) return false;

            stadium.Image = imageUrl;
            _dbcontext.Stadiums.Update(stadium);
            return await _dbcontext.SaveChangesAsync() > 0;
        }
    }
}
