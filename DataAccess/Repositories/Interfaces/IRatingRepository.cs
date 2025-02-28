using BusinessObject.Models;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<bool> AddOrUpdateCommentAsync(RatingModel rating);
        Task<List<Rating>> GetRatingsAsync(int userId);
    }
}
