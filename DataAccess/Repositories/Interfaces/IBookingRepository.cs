using BusinessObject.Models;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<BookingHistoryModel>> GetBookingHistory(int userId);
        Task<List<Booking>> GetAllBookings();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<int> CreateMultipleBookings(Booking booking);
        bool UpdateBookingStatus(int bookingId, string status);
    }
}
