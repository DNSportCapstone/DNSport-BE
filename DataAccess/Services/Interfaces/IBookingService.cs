using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<RevenueReportModel>> GetRevenueReport();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<int> CreateMultipleBookings(MultipleBookingsRequest request);
        bool UpdateBookingStatus(int bookingId, string status);
        void AddTransactionLogAndRevenueTransaction(int bookingId);
    }
}
