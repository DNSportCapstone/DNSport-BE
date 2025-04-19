using BusinessObject.Models;
using DataAccess.Common;
using DataAccess.DAO;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Repositories.Implement;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Services.Implement
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<RevenueReportModel>> GetRevenueReport()
        {
            var bookings = await _bookingRepository.GetAllBookings();
            var result = new List<RevenueReportModel>();
            foreach (var booking in bookings)
            {
                result.Add(new RevenueReportModel
                {
                    Date = booking.BookingDate,
                    Revenue = booking.TotalPrice
                });
            }
            if (result.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    result.Add(new RevenueReportModel
                    {
                        Date = RandomDatetime(),
                        Revenue = new Random().Next(1000, 10000)
                    });
                }
            }
            return result;
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            var result = await _bookingRepository.GetBookingReport();
            if (result.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    result.Add(new BookingReportModel
                    {
                        BookingTime = RandomDatetime(),
                        Type = new string[] { "bóng đá", "cầu lông", "bóng chuyền", "bóng rổ" }[new Random().Next(0, 4)]
                    });
                }
            }
            return result;
        }
        public async Task<int> CreateMultipleBookings(MultipleBookingsRequest request)
        {
            try
            {
                var booking = new Booking
                {
                    UserId = request.Fields.FirstOrDefault()?.UserId,
                    BookingDate = DateTime.UtcNow,
                    Status = Constants.BookingStatus.PendingPayment
                };

                decimal? totalPrice = 0;

                foreach (var field in request.Fields)
                {
                    foreach (var slot in field.SelectedSlots.Where(s => s.IsChoose))
                    {
                        var startTime = slot.Date.Date.Add(TimeSpan.Parse(slot.Time));
                        var endTime = startTime.AddMinutes(slot.Duration);
                        var bookingField = new BookingField
                        {
                            FieldId = field.FieldId,
                            StartTime = startTime,
                            EndTime = endTime,
                            Price = slot.Price,
                            Date = slot.Date.Date,
                        };

                        totalPrice += slot.Price;

                        foreach (var service in slot.Services)
                        {
                            // Get Service price?
                            var bookingFieldService = new BookingFieldService
                            {
                                ServiceId = service.Id,
                                TotalPrice = service.Price * service.Quantity,
                                Quantity = service.Quantity,
                                Price = service.Price
                            };

                            totalPrice += bookingFieldService.TotalPrice;

                            bookingField.BookingFieldServices.Add(bookingFieldService);
                        }
                        booking.BookingFields.Add(bookingField);
                    }
                }
                booking.TotalPrice = totalPrice;

                return await _bookingRepository.CreateMultipleBookings(booking);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool UpdateBookingStatus(int bookingId, string status)
        {
            try
            {
                return _bookingRepository.UpdateBookingStatus(bookingId, status);
            }
            catch
            {
                return false;
            }
        }

        public void AddTransactionLogAndRevenueTransaction(int bookingId)
        {
            try
            {
                _bookingRepository.AddTransactionLogAndRevenueTransaction(bookingId);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> CreateRecurringBookings(RecurringBookingRequest request)
        {
            try
            {
                return await _bookingRepository.CreateRecurringBookings(request);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public decimal GetTotalPriceWithVoucher(int bookingId)
        {
            try
            {
                return _bookingRepository.GetTotalPriceWithVoucher(bookingId);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private DateTime RandomDatetime()
        {
            Random gen = new Random();
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}
