using BusinessObject.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.Common;
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
                                join ud in _dbcontext.UserDetails on u.UserId equals ud.UserId
                                where s.Status != "Rejected"
                                select new StadiumModel
                                {
                                    StadiumId = s.StadiumId,
                                    UserId = s.UserId,
                                    StadiumName = s.StadiumName,
                                    Address = s.Address,
                                    Image = s.Image,
                                    Status = s.Status,
                                    Owner = ud.FullName == string.Empty ? u.Email : $"{ud.FullName} - {u.Email}",
                                }).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<List<StadiumModel>> GetStadiumByName(string stadiumName)
        {
            var normalizedKeyword = Helper.RemoveDiacritics(stadiumName).ToLower();

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
            return result.Where(s => Helper.RemoveDiacritics(s.StadiumName).ToLower().Contains(normalizedKeyword)).ToList();
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

        public async Task<IEnumerable<StadiumLessorModel>> GetStadiumsByLessorIdAsync(int userId)
        {
            var user = await _dbcontext.Set<User>()
                .Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.RoleId != 2)
                throw new Exception("User is not a lessor.");

            var stadiums = await _dbcontext.Set<Stadium>()
                .Where(s => s.UserId == userId)
                .Select(s => new StadiumLessorModel
                {
                    StadiumId = s.StadiumId,
                    StadiumName = s.StadiumName,
                    Address = s.Address,
                    Status = s.Status,
                    LessorName = user.UserDetail.FullName,
                    Email = user.Email
                })
                .ToListAsync();

            return stadiums;
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
        // StadiumRepository.cs
        public async Task<List<StadiumModel>> GetPendingStadiums()
        {
            var pendingStadiums = await _dbcontext.Stadiums
                .Where(s => s.Status == "Pending")
                .Select(s => new StadiumModel
                {
                    StadiumId = s.StadiumId,
                    UserId = s.UserId,
                    StadiumName = s.StadiumName,
                    Address = s.Address,
                    Image = s.Image,
                    Status = s.Status
                })
                .AsNoTracking()
                .ToListAsync();
            return pendingStadiums;
        }

        public async Task<bool> UpdateStadiumStatus(int stadiumId, string newStatus)
        {
            var stadium = await _dbcontext.Stadiums.FirstOrDefaultAsync(s => s.StadiumId == stadiumId && s.Status == "Pending");

            if (stadium == null)
            {
                return false;
            }

            stadium.Status = newStatus;
            await _dbcontext.SaveChangesAsync();
            return true;
        }

    }
}