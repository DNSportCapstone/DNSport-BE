using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interface
{
    public interface IRatingRepository
    {
        Task<bool> AddOrUpdateCommentAsync(RatingModel rating);
        Task<bool> ReplyToRatingAsync(RatingReplyModel replyModel);
        Task<List<Rating>> GetRatingsAsync(int userId);
    }
}
