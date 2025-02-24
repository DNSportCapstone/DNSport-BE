using DataAccess.Interface;
using DataAccess.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace DataAccess.Implement
{
    public class InvoiceService : IInvoiceService
    {
        public byte[] GeneratePdf(InvoiceModel invoice)
        {
            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Hóa đơn #{invoice.Id}").FontSize(20).Bold();
                        col.Item().Text($"Khách hàng: {invoice.CustomerName}");
                        col.Item().Text($"Ngày: {invoice.Date:dd/MM/yyyy}");
                        col.Item().LineHorizontal(1);

                        foreach (var item in invoice.Items)
                        {
                            col.Item().Text($"{item.Description} - {item.Quantity} x {item.UnitPrice} VND");
                        }

                        col.Item().LineHorizontal(1);
                        col.Item().Text($"Tổng cộng: {invoice.TotalAmount} VND").Bold();
                    });
                });
            });

            return pdfDocument.GeneratePdf();
        }
    }
}
