using BusinessObject.Models;
using DataAccess.Common;
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
                                    StadiumName = s.StadiumName,
                                    Description = f.Description
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
            catch (Exception e)
            {
                return false;
            }
        }
        public void AddTransactionLogAndRevenueTransaction(int bookingId)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);

                var transactionLog = new TransactionLog
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    TimeSlot = DateTime.UtcNow,
                    TransactionType = "Banking",
                    ErrorMessage = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var revenueTransaction = new RevenueTransaction
                {
                    BookingId = booking.BookingId,
                    TotalRevenue = booking.TotalPrice,
                    AdminAmount = 90,
                    OwnerAmount = 10,
                    RevenueTransactionDate = DateTime.UtcNow,
                    Status = "Pending",
                };

                _dbContext.TransactionLogs.Add(transactionLog);
                _dbContext.RevenueTransactions.Add(revenueTransaction);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<int> CreateRecurringBookings(RecurringBookingRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var totalPrice = 0m;
                var booking = new Booking
                {
                    UserId = request.UserId,
                    BookingDate = DateTime.UtcNow,
                    Status = Constants.BookingStatus.PendingPayment
                };

                for (int i = 0; i < request.RepeatWeeks; i++)
                {
                    var targetDate = request.StartDate.AddDays(7 * i);
                    while ((int)targetDate.DayOfWeek != request.Weekday)
                        targetDate = targetDate.AddDays(1);

                    foreach (var slot in request.SelectedSlots.Where(s => s.IsChoose))
                    {
                        var startTime = targetDate.Date.Add(TimeSpan.Parse(slot.Time));
                        var endTime = startTime.AddMinutes(slot.Duration);

                        // Check trùng slot
                        var isExist = await _dbContext.BookingFields.AnyAsync(bf =>
                            bf.FieldId == request.FieldId &&
                            bf.Date == targetDate.Date &&
                            ((startTime >= bf.StartTime && startTime < bf.EndTime) ||
                             (endTime > bf.StartTime && endTime <= bf.EndTime) ||
                             (startTime <= bf.StartTime && endTime >= bf.EndTime)));

                        if (isExist)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

                        var bookingField = new BookingField
                        {
                            FieldId = request.FieldId,
                            StartTime = startTime,
                            EndTime = endTime,
                            Date = targetDate.Date,
                            Price = slot.Price
                        };

                        totalPrice += slot.Price;

                        foreach (var serviceId in request.ServiceIds)
                        {
                            var service = await _dbContext.Services.FindAsync(serviceId);
                            if (service != null)
                            {
                                var bfService = new BookingFieldService
                                {
                                    ServiceId = service.ServiceId,
                                    Price = service.Price,
                                    Quantity = 1,
                                    TotalPrice = service.Price
                                };

                                totalPrice += bfService.TotalPrice ?? 0m;
                                bookingField.BookingFieldServices.Add(bfService);
                            }
                        }

                        booking.BookingFields.Add(bookingField);
                    }
                }

                booking.TotalPrice = totalPrice;

                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return booking.BookingId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }
        public decimal GetTotalPriceWithVoucher(int bookingId)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);
                var totalPriceWithVoucher = booking?.TotalPrice ?? 0;
                return totalPriceWithVoucher;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
