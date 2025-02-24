using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interface
{
    public interface IRatingRepository
    {
        Task<bool> AddRatingAsync(RatingModel rating);
        Task<bool> UpdateRatingAsync(int ratingId, RatingModel rating);
        Task<List<Rating>> GetRatingsAsync(int userId);
    }
}
