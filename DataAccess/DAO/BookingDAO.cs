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
            var result = await (from b in _dbContext.Bookings
                                join bf in _dbContext.BookingFields on b.BookingId equals bf.BookingId
                                join bs in _dbContext.BookingFieldServices on bf.BookingFieldId equals bs.BookingFieldId
                                join s in _dbContext.Services on bs.ServiceId equals s.ServiceId
                                join c in _dbContext.ServiceCategories on s.CategoryId equals c.CategoryId
                                join u in _dbContext.Users on b.UserId equals u.UserId
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
