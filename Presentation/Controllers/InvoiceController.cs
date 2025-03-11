using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService, IUserService userService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var pdfBytes = await _invoiceService.GeneratePdf(id);
            return File(pdfBytes, "application/pdf", $"Hoa_don_{id}.pdf");
        }
    }
}
