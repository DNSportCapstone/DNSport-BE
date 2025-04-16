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
            foreach (var booking in bookings.Where(b => b.Status == "Success").ToList())
            {
                result.Add(new RevenueReportModel
                {
                    Date = booking.BookingDate,
                    Revenue = booking.TotalPrice
                });
            }
            return result;
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            var result = await _bookingRepository.GetBookingReport();
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

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
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

        public Task<List<DenounceModel>> GetAllDenounce()
        {
            var result = _bookingRepository.GetAllDenounce();
            return result;
        }

        public async Task<BookingInvoiceModel> GetBookingInvoice(int id)
        {
            var result = await _bookingRepository.GetBookingInvoice(id);

            return result;
        }

        public async Task<int> CreateBookingReport(ReportRequest bookingReport)
        {
            try
            {
                return await _bookingRepository.CreateBookingReport(bookingReport);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
