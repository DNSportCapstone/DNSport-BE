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

        private DateTime RandomDatetime()
        {
            Random gen = new Random();
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        public async Task<BookingInvoiceModel> GetBookingInvoice(int id)
        {
            var result = await _bookingRepository.GetBookingInvoice(id);

            return result;
        }
    }
}
