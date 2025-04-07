using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

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
            return result;
        }

        public async Task<List<BookingReportModel>> GetBookingReport()
        {
            var result = await _bookingRepository.GetBookingReport();
            return result;
        }

        public Task<List<DenounceModel>> GetAllDenounce()
        {
            throw new NotImplementedException();
        }

        public async Task<BookingInvoiceModel> GetBookingInvoice(int id)
        {
            var result = await _bookingRepository.GetBookingInvoice(id);

            return result;
        }
    }
}
