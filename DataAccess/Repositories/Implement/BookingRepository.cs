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
        private readonly Db12353Context _dbContext;
        public BookingRepository(BookingDAO bookingDAO, Db12353Context dbcontext)
        {
            _bookingDAO = bookingDAO;
            _dbContext = dbcontext;
        }
        public async Task<List<BookingHistoryModel>> GetBookingHistory(int userId)
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            var result = await (from b in _dbContext.Bookings
                                join u in _dbContext.Users on b.UserId equals u.UserId
                                join bf in _dbContext.BookingFields on b.BookingId equals bf.BookingId
                                join f in _dbContext.Fields on bf.FieldId equals f.FieldId
                                join s in _dbContext.Stadiums on f.StadiumId equals s.StadiumId
                                join d in _dbContext.Denounces on b.BookingId equals d.BookingId into denounceJoin
                                from d in denounceJoin.DefaultIfEmpty()
                                where b.Status == Constants.BookingStatus.Success && b.UserId == userId
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
                                    Description = f.Description,
                                    IsReport = d != null ? true : false
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
                                 join bf in _dbContext.BookingFields on b.BookingId equals bf.BookingId
                                 join f in _dbContext.Fields on bf.FieldId equals f.FieldId
                                 join s in _dbContext.Stadiums on f.StadiumId equals s.StadiumId
                                 where b.BookingId == bookingId
                                 select new BookingInvoiceModel
                                 {
                                     BookingId = b.BookingId,
                                     Date = (DateTime)b.BookingDate,
                                     StadiumName = s.StadiumName,
                                     StadiumAddress = s.Address,
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

        public async Task<int> CreateBookingReport(ReportRequest bookingReport)
        {
            try
            {
                var isExist = await _dbContext.Denounces.AnyAsync(b => b.BookingId == bookingReport.BookingId);
                if (isExist)
                {
                    return 0;
                }
                var lessor = await (from b in _dbContext.Bookings
                                    join bf in _dbContext.BookingFields on b.BookingId equals bf.BookingId
                                    join f in _dbContext.Fields on bf.FieldId equals f.FieldId
                                    join s in _dbContext.Stadiums on f.StadiumId equals s.StadiumId
                                    join u in _dbContext.Users on b.UserId equals u.UserId
                                    where b.BookingId == bookingReport.BookingId
                                    select u).FirstOrDefaultAsync();
                var report = new Denounce()
                {
                    SendId = bookingReport.UserId,
                    ReceiveId = lessor.UserId,
                    BookingId = bookingReport.BookingId,
                    Description = bookingReport.Description,
                    DenounceTime = DateTime.UtcNow.AddHours(7),
                    Status = "Pending"
                };

                await _dbContext.Denounces.AddAsync(report);
                await _dbContext.SaveChangesAsync();

                if (bookingReport.ImageUrl != null)
                {
                    var bookingImage = new DenounceImage()
                    {
                        DenounceId = report.DenounceId,
                        Url = bookingReport.ImageUrl
                    };
                    await _dbContext.DenounceImages.AddAsync(bookingImage);
                    await _dbContext.SaveChangesAsync();
                }
                return report.DenounceId;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<List<DenounceModel>> GetAllDenounce()
        {
            var result = await (from d in _dbContext.Denounces
                                join di in _dbContext.DenounceImages on d.DenounceId equals di.DenounceId into denounceImageJoin
                                from di in denounceImageJoin.DefaultIfEmpty()
                                join u in _dbContext.Users on d.SendId equals u.UserId into userJoin
                                from u in userJoin.DefaultIfEmpty()
                                join ud in _dbContext.UserDetails on u.UserId equals ud.UserId into userDetailsJoin
                                from ud in userDetailsJoin.DefaultIfEmpty()
                                join b in _dbContext.Bookings on d.BookingId equals b.BookingId into bookingJoin
                                from b in bookingJoin.DefaultIfEmpty()
                                join bf in _dbContext.BookingFields on b.BookingId equals bf.BookingId into bookingFieldJoin
                                from bf in bookingFieldJoin.DefaultIfEmpty()
                                join f in _dbContext.Fields on bf.FieldId equals f.FieldId into fieldJoin
                                from f in fieldJoin.DefaultIfEmpty()
                                join s in _dbContext.Stadiums on f.StadiumId equals s.StadiumId into stadiumJoin
                                from s in stadiumJoin.DefaultIfEmpty()

                                select new DenounceModel
                                {
                                    DenounceId = d.DenounceId,
                                    BookingId = d.BookingId,
                                    Description = d.Description,
                                    DenounceTime = d.DenounceTime,
                                    Status = d.Status,
                                    UserName = ud != null ? ud.FullName : null,
                                    BookingDate = (DateTime)b.BookingDate,
                                    ImageUrl = di.Url,
                                    UserId = (int)d.SendId,
                                    StadiumName = s.StadiumName,
                                    Email = u.Email,
                                    PhoneNumber = ud.PhoneNumber
                                }).AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<List<TransactionLogModel>> GetTransactionLog(int userId)
        {
            var result = await (from t in _dbContext.TransactionLogs
                                join b in _dbContext.Bookings on t.BookingId equals b.BookingId into bookingJoin
                                from b in bookingJoin.DefaultIfEmpty()
                                where t.UserId == userId
                                select new TransactionLogModel
                                {
                                    LogId = t.LogId,
                                    UserId = t.UserId,
                                    BookingId = t.BookingId,
                                    TimeSlot = t.TimeSlot,
                                    TransactionType = t.TransactionType,
                                    ErrorMessage = t.ErrorMessage,
                                    CreatedAt = t.CreatedAt,
                                    UpdatedAt = t.UpdatedAt,
                                    Amount = b.TotalPrice.ToString()
                                }).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<int> SetReportStatus(int id, string status)
        {
            try
            {
                var report = await _dbContext.Denounces.FindAsync(id);
                if (report == null)
                {
                    return 0;
                }
                report.Status = status;
                await _dbContext.SaveChangesAsync();
                return report.DenounceId;
            }
            catch (Exception)
            {
                return 0;
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
                    TimeSlot = DateTime.UtcNow.AddHours(7),
                    TransactionType = "Banking",
                    ErrorMessage = string.Empty,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7),
                };

                var lessorPercentage = booking.BookingFields.First().Field?.Stadium?.RevenueSharings?.First().LessorPercentage ?? 90;

                var revenueTransaction = new RevenueTransaction
                {
                    BookingId = booking.BookingId,
                    TotalRevenue = booking.TotalPrice,
                    AdminAmount = booking.TotalPrice * (100-lessorPercentage)/100,
                    OwnerAmount = booking.TotalPrice * lessorPercentage/100,
                    RevenueTransactionDate = DateTime.UtcNow.AddHours(7),
                    Status = "Success",
                };

                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Deposit = booking.TotalPrice,
                    PaymentTime = DateTime.UtcNow.AddHours(7),
                    Status = "Success"
                };

                _dbContext.TransactionLogs.Add(transactionLog);
                _dbContext.RevenueTransactions.Add(revenueTransaction);
                _dbContext.Payments.Add(payment);
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
                    BookingDate = DateTime.UtcNow.AddHours(7),
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
