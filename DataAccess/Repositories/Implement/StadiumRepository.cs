using BusinessObject.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.DAO;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implement
{
    public class StadiumRepository : IStadiumRepository
    {
        private readonly Cloudinary _cloudinary;
        private readonly StadiumDAO _stadiumDAO;
        private readonly Db12353Context _dbcontext;
        public StadiumRepository(IOptions<CloudinarySettings> config, StadiumDAO stadiumDAO, Db12353Context dbcontext)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _stadiumDAO = stadiumDAO;
            _dbcontext = dbcontext;
        }

        public async Task<List<StadiumModel>> GetStadiumData()
        {
            var result = await (from s in _dbcontext.Stadiums
                                join u in _dbcontext.Users on s.UserId equals u.UserId
                                select new StadiumModel
                                {
                                    StadiumId = s.StadiumId,
                                    UserId = s.UserId,
                                    StadiumName = s.StadiumName,
                                    Address = s.Address,
                                    Image = s.Image,
                                    Status = s.Status,
                                    Owner = u.Email
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

        public async Task<int> DisableStadium(int id, string status)
        {
            return await _stadiumDAO.DisableStadium(id, status);
        }
        public async Task<List<StadiumModel>> GetStadiumsByUserId(int userId)
        {
            var result = await (from s in _dbcontext.Stadiums
                                join u in _dbcontext.Users on s.UserId equals u.UserId
                                where s.UserId == userId
                                select new StadiumModel
                                {
                                    StadiumId = s.StadiumId,
                                    UserId = s.UserId,
                                    StadiumName = s.StadiumName,
                                    Address = s.Address,
                                    Image = s.Image,
                                    Status = s.Status,
                                    Owner = u.Email
                                }).AsNoTracking().ToListAsync();

            return result;
        }

    }
}
