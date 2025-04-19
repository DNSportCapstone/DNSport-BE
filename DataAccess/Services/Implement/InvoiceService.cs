using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DataAccess.Services.Implement
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUserRepository _iUserRepository;
        private readonly IBookingService _iBookingService;
        public InvoiceService(IUserRepository iUserRepository, IBookingService iBookingService)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _iUserRepository = iUserRepository;
            _iBookingService = iBookingService;
        }

        public async Task<byte[]> GeneratePdf(int bookingId)
        {
            var user = await _iUserRepository.GetUserByBookingId(bookingId);
            var booking = await _iBookingService.GetBookingInvoice(bookingId);
            var invoice = new InvoiceModel
            {
                Id = user.UserId,
                CustomerName = user.UserDetail?.FullName ?? "N/A",
                Email = user.Email,
                PhoneNumber = user.UserDetail?.PhoneNumber ?? "N/A",
                Address = user.UserDetail?.Address ?? "N/A",
                Date = DateTime.Now,
                Items = booking.ItemBooking.Concat(booking.ItemService).ToList()
            };

            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Text("HÓA ĐƠN").FontSize(24).Bold().FontColor(Colors.Blue.Medium); // Changed to RelativeItem
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(3).Text($"Ngày: {invoice.Date:dd/MM/yyyy}").FontSize(12);
                        col.Item().PaddingVertical(3).Text($"Khách hàng: {invoice.CustomerName}").FontSize(14).Bold();
                        col.Item().PaddingVertical(3).Text($"Số điện thoại: {invoice.PhoneNumber}").FontSize(14);
                        col.Item().PaddingVertical(3).Text($"Địa chỉ: {invoice.Address}").FontSize(14);
                        col.Item().PaddingVertical(3).Text($"Email: {invoice.Email}").FontSize(14);
                        col.Item().PaddingVertical(5);

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
                                header.Cell().Element(HeaderCellStyle).Text("STT").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).Text("Dịch vụ").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).Text("Số lượng").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).Text("Giá tiền").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).Text("Thành tiền").Bold().FontColor(Colors.White);
                            });

                            int index = 1;
                            foreach (var item in invoice.Items)
                            {
                                bool isEvenRow = index % 2 == 0;
                                table.Cell().Element(cell => CellStyle(cell, isEvenRow)).Text(index++.ToString());
                                table.Cell().Element(cell => CellStyle(cell, isEvenRow)).Text(item.Description);
                                table.Cell().Element(cell => CellStyle(cell, isEvenRow)).Text(item.Quantity.ToString());
                                table.Cell().Element(cell => CellStyle(cell, isEvenRow)).Text($"{item.UnitPrice:N0} VND");
                                table.Cell().Element(cell => CellStyle(cell, isEvenRow)).Text($"{item.TotalPrice:N0} VND");
                            }
                        });

                        col.Item().LineHorizontal(1);
                        col.Item().AlignRight().Text($"Tổng cộng: {invoice.TotalAmount:N0} VND").Bold().FontSize(16);
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("Cảm ơn quý khách! Hẹn gặp lại!").Italic().FontSize(12);
                });
            });

            return pdfDocument.GeneratePdf();
        }

        private IContainer CellStyle(IContainer container, bool isEvenRow)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Padding(2)
                .Background(isEvenRow ? Colors.Grey.Lighten3 : Colors.White)
                .AlignCenter();
        }

        private IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Padding(2)
                .Background(Colors.Blue.Medium)
                .AlignCenter();
        }
    }
}
