using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDAO _bookingDAO;
        private readonly Db12353Context _dbContext;
        public BookingRepository(BookingDAO bookingDAO, Db12353Context context)
        {
            _bookingDAO = bookingDAO;
            _dbContext = context;
        }
        public async Task<List<BookingHistoryModel>> GetBookingHistory(int userId)
        {
            var _dbContext = new Db12353Context();

            var result = await (from b in _dbContext.Bookings
                                join u in _dbContext.Users
                                on b.UserId equals u.UserId
                                join bf in _dbContext.BookingFields
                                on b.BookingId equals bf.BookingId
                                join f in _dbContext.Fields
                                on bf.FieldId equals f.FieldId
                                join s in _dbContext.Stadiums
                                on f.StadiumId equals s.StadiumId
                                where b.UserId == userId
                                select new BookingHistoryModel
                                {
                                    BookingId = b.BookingId,
                                    TotalPrice = b.TotalPrice,
                                    BookingDate = b.BookingDate,
                                    Status = b.Status,
                                    FieldId = bf.FieldId,
                                    StartTime = bf.StartTime,
                                    EndTime = bf.EndTime,
                                    StadiumName = s.StadiumName
                                }).AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<List<Booking>> GetAllBookings()
        {
            return await _bookingDAO.GetAllBooking();
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            return await _bookingDAO.GetBookingReport();
        }

        public async Task<BookingInvoiceModel> GetBookingInvoice(int bookingId)
        {
            var booking = await (from b in _dbContext.Bookings
                                 where b.BookingId == bookingId
                                 select new BookingInvoiceModel
                                 {
                                     BookingId = b.BookingId,
                                     Date = (DateTime)b.BookingDate,
                                     ItemBooking = (from bf in _dbContext.BookingFields
                                                    join f in _dbContext.Fields
                                                    on bf.FieldId equals f.FieldId
                                                    where bf.BookingId == bookingId
                                                    select new InvoiceItem
                                                    {
                                                        Description = f.Description,
                                                        Quantity = 1,
                                                        UnitPrice = (decimal)(b.BookingDate.Value.Hour >= 18 ? f.NightPrice : f.DayPrice)
                                                    }).AsNoTracking()
                                                      .ToList(),
                                     ItemService = (from bf in _dbContext.BookingFields
                                                    join bs in _dbContext.BookingFieldServices
                                                    on bf.BookingFieldId equals bs.BookingFieldId
                                                    join s in _dbContext.Services
                                                    on bs.ServiceId equals s.ServiceId
                                                    where bf.BookingId == bookingId
                                                    select new InvoiceItem
                                                    {
                                                        Description = s.ServiceName,
                                                        Quantity = (int)bs.Quantity,
                                                        UnitPrice = (decimal)s.Price
                                                    }).AsNoTracking()
                                                      .ToList()
                                 }).AsNoTracking().FirstOrDefaultAsync();
            return booking;
        }
    }
}
