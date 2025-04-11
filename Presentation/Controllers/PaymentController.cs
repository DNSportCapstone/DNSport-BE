using DataAccess.Common;
using DataAccess.Model;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly VnpayPayment _vnpayPayment;
        private readonly IBookingService _bookingService;
        public PaymentController(IVnpay vnpay, VnpayPayment vnpayPayment, IBookingService bookingService)
        {
            _vnpay = vnpay;
            _vnpayPayment = vnpayPayment;
            _bookingService = bookingService;
        }

        [HttpPost("create-payment-url")]
        public ActionResult<string> CreatePaymentUrl([FromBody] PaymentRequestModel payment)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)payment.Amount,
                    Description = "thanh toan",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                var result = Created(paymentUrl, paymentUrl);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vnpay-return")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess && int.TryParse(paymentResult.Description, out int bookingId))
                    {
                        _bookingService.UpdateBookingStatusAsync(bookingId, Constants.BookingStatus.Paid);
                        return Ok();
                    }
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        [HttpPost("payment-url/multiple-booking")]
        public ActionResult<string> CreatePaymentUrlForMultipleBooking([FromBody] PaymentRequestModel payment)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)payment.Amount,
                    Description = payment.BookingId.ToString(),
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                var result = Created(paymentUrl, paymentUrl);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
