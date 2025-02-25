using BusinessObject.Models;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implement
{
    public class RatingRepository : IRatingRepository
    {
        private readonly Db12353Context _context = new();

        public async Task<bool> AddOrUpdateCommentAsync(RatingModel rating)
        {
            if (rating.RatingValue.HasValue && (rating.RatingValue < 1 || rating.RatingValue > 5))
            {
                throw new Exception("RatingValue must be between 1 and 5.");
            }
            // Kiểm tra User tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.UserId == rating.UserId);
            if (!userExists)
            {
                throw new Exception("User does not exist");
            }

            // Kiểm tra Booking có tồn tại và có status "Success"
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == rating.BookingId &&
                                          b.UserId == rating.UserId &&
                                          b.Status == "Success");

            if (booking == null)
            {
                throw new Exception("Only successful bookings can be rated.");
            }

            // Kiểm tra thời gian hiện tại có qua thời gian đặt sân chưa
            var bookingField = await _context.BookingFields
                .FirstOrDefaultAsync(bf => bf.BookingId == rating.BookingId);

            if (bookingField == null)
            {
                throw new Exception("Invalid booking field data.");
            }

            if (DateTime.Now < bookingField.EndTime)
            {
                throw new Exception("You can only rate after the booked time has passed.");
            }

            // check đã đánh giá sao chưa
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.BookingId == rating.BookingId);

            if (existingRating != null)
            {
                // Nếu rating đã tồn tại, không cho phép thay đổi RatingValue, chỉ cho phép cập nhật Comment
                if (rating.RatingValue != null && existingRating.RatingValue != rating.RatingValue)
                {
                    throw new Exception("You have already rated this booking. You can only update your comment.");
                }

                // Cập nhật Comment
                existingRating.Comment = rating.Comment;
                existingRating.Time = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }

            //chưa có rating, thêm mới 
            var newRating = new Rating
            {
                UserId = rating.UserId,
                BookingId = rating.BookingId,
                RatingValue = rating.RatingValue ?? 0, // null thì là 0
                Comment = rating.Comment,
                Time = DateTime.Now
            };

            _context.Ratings.Add(newRating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Rating>> GetRatingsAsync(int userId)
        {
            return await _context.Ratings
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}
