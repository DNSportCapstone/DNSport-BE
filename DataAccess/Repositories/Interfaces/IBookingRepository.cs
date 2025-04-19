using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<BookingHistoryModel>> GetBookingHistory(int userId);
        Task<List<Booking>> GetAllBookings();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<BookingInvoiceModel> GetBookingInvoice(int bookingId);
        Task<int> CreateMultipleBookings(Booking booking);
        bool UpdateBookingStatus(int bookingId, string status);
        Task<int> CreateBookingReport(ReportRequest bookingReport);
        Task<List<DenounceModel>> GetAllDenounce();
        Task<List<TransactionLogModel>> GetTransactionLog(int userId);
        Task<int> SetReportStatus(int id, string status);
        void AddTransactionLogAndRevenueTransaction(int bookingId);
        Task<int> CreateRecurringBookings(RecurringBookingRequest request);
        decimal GetTotalPriceWithVoucher(int bookingId);
    }
}
