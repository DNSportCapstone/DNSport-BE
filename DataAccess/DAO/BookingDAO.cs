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
                                join bf in context.BookingFields on b.BookingId equals bf.BookingId into bfGroup
                                from bf in bfGroup.DefaultIfEmpty()
                                join f in context.Fields on bf.BookingFieldId equals f.FieldId into fGroup
                                from f in fGroup.DefaultIfEmpty() 
                                join s in context.Sports on f.SportId equals s.SportId into sGroup
                                from s in sGroup.DefaultIfEmpty()
                                join u in context.Users on b.UserId equals u.UserId into uGroup
                                from u in uGroup.DefaultIfEmpty()
                                where b.Status == "Success"
                                select new BookingReportModel
                                {
                                    UserId = u != null ? u.UserId : 0,
                                    UserName = u != null ? u.Email : string.Empty,
                                    BookingTime = b.BookingDate,
                                    Type = s != null ? s.SportName : string.Empty,
                                    SportId = s != null ? s.SportId : 0
                                }).AsNoTracking().ToListAsync();

            return result;
        }
    }
}
