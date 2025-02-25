using DataAccess.Interface;
using DataAccess.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DataAccess.Implement
{
    public class InvoiceService : IInvoiceService
    {
        public InvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePdf(InvoiceModel invoice)
        {
            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().AlignCenter().Text("HÓA ĐƠN").FontSize(20).Bold().FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Khách hàng: {invoice.CustomerName}").FontSize(14).Bold();
                        col.Item().Text($"Ngày: {invoice.Date:dd/MM/yyyy}").FontSize(12);
                        col.Item().LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("STT").Bold();
                                header.Cell().Element(CellStyle).Text("Mô tả").Bold();
                                header.Cell().Element(CellStyle).Text("Số lượng").Bold();
                                header.Cell().Element(CellStyle).Text("Đơn giá").Bold();
                                header.Cell().Element(CellStyle).Text("Thành tiền").Bold();
                            });

                            int index = 1;
                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(item.Description);
                                table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text($"{item.UnitPrice:N0} VND");
                                table.Cell().Element(CellStyle).Text($"{item.TotalPrice:N0} VND");
                            }
                        });

                        col.Item().LineHorizontal(1);

                        // Tổng tiền
                        col.Item().AlignRight().Text($"Tổng cộng: {invoice.TotalAmount:N0} VND").Bold().FontSize(16);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Cảm ơn quý khách đã sử dụng dịch vụ!").Italic().FontSize(12);
                });
            });

            return pdfDocument.GeneratePdf();
        }

        private IContainer CellStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .Padding(5);
        }
    }
}
