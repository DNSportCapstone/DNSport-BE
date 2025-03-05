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
        Task<bool> AddRatingAsync(RatingModel rating);
        Task<bool> AddReplyAsync(RatingReplyModel replyModel);
        Task<Rating> GetRatingByBookingAsync(int bookingId, int userId);
        Task<bool> DetectAndReportCommentAsync(int ratingId, string comment);
        Task<List<int>> GetReportedCommentsAsync();
        Task<List<Rating>> GetCommentsByStadiumAsync(int stadiumId);
    }
}
