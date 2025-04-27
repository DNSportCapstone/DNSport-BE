using BusinessObject.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DataAccess.Repositories.Implement
{
    public class RatingRepository : IRatingRepository
    {
        private readonly Db12353Context _context;

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



        public async Task<List<RatingModel>> GetCommentsByFieldIdAsync(int fieldId)
        {
            var comments = await (from r in _context.Ratings
                                  join b in _context.Bookings on r.BookingId equals b.BookingId
                                  join bf in _context.BookingFields on b.BookingId equals bf.BookingId
                                  join u in _context.Users on r.UserId equals u.UserId
                                  join ud in _context.UserDetails on u.UserId equals ud.UserId
                                  where bf.FieldId == fieldId
                                        && !string.IsNullOrEmpty(r.Comment)
                                  orderby r.Time descending
                                  select new RatingModel
                                  {
                                      RatingId = r.RatingId,
                                      BookingId = b.BookingId,
                                      UserId = u.UserId,
                                      FullName = ud.FullName,
                                      RatingValue = r.RatingValue,
                                      Comment = r.Comment,
                                      Time = r.Time,
                                      Reply = r.Reply,
                                      ReplyTime = r.ReplyTime
                                  }).ToListAsync();

            return comments;
        }



    }
}

