using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadInvoice(int id)
        {
            var invoice = new InvoiceModel
            {
                Id = id,
                CustomerName = "Nguyễn Văn A",
                Date = DateTime.Now,
                Items = new List<InvoiceItem>
                {
                    new() { Description = "Sản phẩm A", Quantity = 2, UnitPrice = 200000 },
                    new() { Description = "Sản phẩm B", Quantity = 1, UnitPrice = 100000 },
                }
            };

            var pdfBytes = _invoiceService.GeneratePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }
    }
}
