using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDAO _bookingDAO;
        private Db12353Context _dbContext;
        public BookingRepository(BookingDAO bookingDAO, Db12353Context dbcontext)
        {
            _bookingDAO = bookingDAO;
            _dbContext = dbcontext;
        }
        public async Task<List<BookingHistoryModel>> GetBookingHistory(int userId)
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
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
        public async Task<int> CreateMultipleBookings(Booking booking)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Check slots are all exist
                foreach (var bookingField in booking.BookingFields)
                {
                        var isExist = await _dbContext.BookingFields
                            .AnyAsync(bf =>
                                bf.FieldId == bookingField.FieldId &&
                                bf.Date == bookingField.Date &&
                                ((bookingField.StartTime >= bf.StartTime && bookingField.StartTime < bf.EndTime) ||
                                 (bookingField.EndTime > bf.StartTime && bookingField.EndTime <= bf.EndTime) ||
                                 (bookingField.StartTime <= bf.StartTime && bookingField.EndTime >= bf.EndTime))
                            );

                        if (isExist)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }
                }
                _dbContext.Add(booking);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return booking.BookingId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }
        public bool UpdateBookingStatus(int bookingId, string status)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);

                if (booking == null)
                {
                    return false;
                }

                booking.Status = status;
                _dbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
