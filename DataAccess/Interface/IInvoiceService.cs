using DataAccess.Model;

namespace DataAccess.Interface
{
    public interface IInvoiceService
    {
        byte[] GeneratePdf(InvoiceModel invoice);
    }
}
