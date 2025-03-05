using BusinessObject.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DataAccess.Repositories.Implement
{
    public class RatingRepository : IRatingRepository
    {
        private readonly Db12353Context _context;

        private static HashSet<int> _reportedComments = new HashSet<int>(); // Lưu comment vi phạm trong bộ nhớ

        //Danh sách từ cấm
        private readonly List<string> _bannedWords = new List<string>
        {
            "chửi", "tục", "lừa đảo", "scam", "fake"
        };

        public RatingRepository(Db12353Context context)
        {
            _context = context;
        }

        // Thêm hoặc cập nhật rating
        public async Task<bool> AddRatingAsync(RatingModel rating)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == rating.BookingId &&
                                          b.UserId == rating.UserId &&
                                          b.Status == "Success");

            if (booking == null)
            {
                throw new Exception("Only successful bookings can be rated.");
            }

            // Kiểm tra EndTime 
            var bookingField = await _context.BookingFields
                .FirstOrDefaultAsync(bf => bf.BookingId == rating.BookingId);

            if (bookingField == null || DateTime.Now < bookingField.EndTime)
            {
                throw new Exception("You can only rate after the booked time has passed.");
            }

            // Kiểm tra đã rating chưa
            var existingRating = await _context.Ratings
                .AnyAsync(r => r.BookingId == rating.BookingId && r.UserId == rating.UserId);

            if (existingRating)
            {
                throw new Exception("You can only rate once per booking.");
            }

            var newRating = new Rating
            {
                UserId = rating.UserId,
                BookingId = rating.BookingId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                Time = DateTime.Now
            };

            _context.Ratings.Add(newRating);
            await _context.SaveChangesAsync();
            return true;
        }


        //Thêm reply 
        public async Task<bool> AddReplyAsync(RatingReplyModel replyModel)
        {
            var rating = await _context.Ratings.FindAsync(replyModel.RatingId);
            if (rating == null)
            {
                throw new Exception("Rating not found.");
            }

            if (!string.IsNullOrEmpty(rating.Reply))
            {
                throw new Exception("You can only reply once to a comment.");
            }

            rating.Reply = replyModel.Reply;
            rating.ReplyTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        //Kiểm tra đã rating chưa
        public async Task<Rating> GetRatingByBookingAsync(int bookingId, int userId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.BookingId == bookingId && r.UserId == userId);
        }

        public async Task<bool> DetectAndReportCommentAsync(int ratingId, string comment)
        {
            var rating = await _context.Ratings.FindAsync(ratingId);
            if (rating == null)
            {
                throw new Exception("Rating not found.");
            }

            // Kiểm tra chứa từ cấm không
            if (_bannedWords.Any(word => Regex.IsMatch(comment, $"\\b{word}\\b", RegexOptions.IgnoreCase)))
            {
                _reportedComments.Add(ratingId); // Tự động ẩn comment
                return true;
            }

            return false;
        }

        //Lấy danh sách comment bị ẩn
        public async Task<List<int>> GetReportedCommentsAsync()
        {
            return _reportedComments.ToList();
        }

        //Lấy danh sách comment của một sân
        public async Task<List<Rating>> GetCommentsByStadiumAsync(int stadiumId)
        {
            return await _context.Ratings
                .Where(r => _context.Bookings
                    .Any(b => b.BookingId == r.BookingId &&
                              _context.BookingFields
                                  .Any(bf => bf.BookingId == b.BookingId && bf.Field.StadiumId == stadiumId)) &&
                            !_reportedComments.Contains(r.RatingId)) // Ẩn comment vi phạm
                .OrderByDescending(r => r.Time)
                .ToListAsync();
        }
    }
}

