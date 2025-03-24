using BusinessObject.Models;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class BookingDAO
    {
        public async Task<List<Booking>> GetAllBooking()
        {
            using var context = new Db12353Context();
            return await context.Bookings.ToListAsync();
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            using var context = new Db12353Context();
            var result = await (from b in context.Bookings
                                join bf in context.BookingFields on b.BookingId equals bf.BookingId
                                join bs in context.BookingFieldServices on bf.BookingFieldId equals bs.BookingFieldId
                                join s in context.Services on bs.ServiceId equals s.ServiceId
                                join c in context.ServiceCategories on s.CategoryId equals c.CategoryId
                                join u in context.Users on b.UserId equals u.UserId
                                select new BookingReportModel
                                {
                                    UserId = u.UserId,
                                    UserName = u.Email,
                                    BookingTime = b.BookingDate,
                                    Type = c.CategoryName
                                }).AsNoTracking().ToListAsync();
            return result;
        }
    }
}
