using BusinessObject.Models;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class BookingDAO
    {
        private readonly Db12353Context _dbContext;
        public BookingDAO(Db12353Context dbcontext)
        {
            _dbContext = dbcontext;
        }
        public async Task<List<Booking>> GetAllBooking()
        {
            return await _dbContext.Bookings.ToListAsync();
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            using var context = new Db12353Context();
            var result = await (from b in context.Bookings
                                join bf in context.BookingFields on b.BookingId equals bf.BookingId
                                join f in context.Fields on bf.BookingFieldId equals f.FieldId
                                join s in context.Sports on f.SportId equals s.SportId
                                join u in context.Users on b.UserId equals u.UserId
                                where b.Status == "Success"
                                select new BookingReportModel
                                {
                                    UserId = u.UserId,
                                    UserName = u.Email,
                                    BookingTime = b.BookingDate,
                                    Type = s.SportName
                                }).AsNoTracking().ToListAsync();
            return result;
        }
    }
}
