namespace DataAccess.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<byte[]> GeneratePdf(int bookingId);
    }
}
