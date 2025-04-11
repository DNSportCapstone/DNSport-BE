using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<RevenueReportModel>> GetRevenueReport();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<int> CreateMultipleBookings(MultipleBookingsRequest request);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
    }
}
