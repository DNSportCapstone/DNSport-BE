using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<bool> AddRatingAsync(RatingModel rating);
        Task<bool> AddReplyAsync(RatingReplyModel replyModel);
        Task<List<Rating>> GetCommentsByFieldIdAsync(int stadiumId);
        Task<Rating> GetRatingByBookingAsync(int bookingId, int userId);
    }
}