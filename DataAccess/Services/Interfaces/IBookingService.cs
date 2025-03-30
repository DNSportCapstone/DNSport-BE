using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<RevenueReportModel>> GetRevenueReport();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<BookingInvoiceModel> GetBookingInvoice(int id);
    }
}
