using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<RevenueReportModel>> GetRevenueReport();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<List<DenounceModel>> GetAllDenounce();
        Task<BookingInvoiceModel> GetBookingInvoice(int id);
        Task<int> CreateMultipleBookings(MultipleBookingsRequest request);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
    }
}
