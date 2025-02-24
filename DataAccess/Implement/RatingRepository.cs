using BusinessObject.Models;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class RatingRepository : IRatingRepository
    {
        private readonly Db12353Context _context = new ();

        public async Task<bool> AddRatingAsync(RatingModel rating)
        {
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

            // Xóa rating cũ nếu tồn tại
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.BookingId == rating.BookingId);

            if (existingRating != null)
            {
                _context.Ratings.Remove(existingRating);
                await _context.SaveChangesAsync(); // Xóa rating cũ trước khi thêm mới
            }

            // Thêm rating mới vào database
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



        // Cập nhật rating
        public async Task<bool> UpdateRatingAsync(int ratingId, RatingModel rating)
        {
            var existingRating = await _context.Ratings.FindAsync(ratingId);
            if (existingRating == null)
            {
                throw new Exception("Rating not found");
            }

            existingRating.RatingValue = rating.RatingValue;
            existingRating.Comment = rating.Comment;
            existingRating.Time = DateTime.Now; // update timne

            await _context.SaveChangesAsync();
            return true;
        }

        // get rating list
        public async Task<List<Rating>> GetRatingsAsync(int userId)
        {
            return await _context.Ratings
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}
