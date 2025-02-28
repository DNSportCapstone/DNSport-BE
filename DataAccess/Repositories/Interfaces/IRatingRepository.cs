using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interface
{
    public interface IRatingRepository
    {
       
        Task<bool> AddOrUpdateRatingAsync(RatingModel rating);
        Task<bool> AddReplyAsync(RatingReplyModel replyModel);
        Task<Rating> GetRatingByBookingAsync(int bookingId, int userId);
        Task<bool> DetectAndReportCommentAsync(int ratingId, string comment);
        Task<List<int>> GetReportedCommentsAsync();
        Task<List<Rating>> GetCommentsByStadiumAsync(int stadiumId);
    }
}
