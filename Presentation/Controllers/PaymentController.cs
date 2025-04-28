using BusinessObject.Models;
using CloudinaryDotNet;
using DataAccess.Common;
using DataAccess.Model;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System.Net.Http.Headers;
using System.Text.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly PayOS _payOS;
        public PaymentController(IVnpay vnpay, VnpayPayment vnpayPayment, IBookingService bookingService, IHttpClientFactory httpClientFactory, IConfiguration configuration, PayOS payOS)
        {
            _vnpay = vnpay;
            _vnpayPayment = vnpayPayment;
            _bookingService = bookingService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _payOS = payOS;
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
                        if (_bookingService.UpdateBookingStatus(bookingId, Constants.BookingStatus.Paid))
                        {
                            _bookingService.AddTransactionLogAndRevenueTransaction(bookingId);
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("Thanh toán thất bại");
                        }
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
        [HttpPost("payment-url/recurring-booking")]
        public ActionResult<string> CreatePaymentUrlForRecurringBooking([FromBody] PaymentRequestModel payment)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                var totalPriceWithVoucher = _bookingService.GetTotalPriceWithVoucher(payment.BookingId);
                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)totalPriceWithVoucher,
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

        [HttpPost("payos-create-payment/multiple-booking")]
        public async Task<IActionResult> CreatePaymentPay([FromBody] PaymentRequestModel model)
        {
            var payOSAPIKey = _configuration["PayOS:APIKey"];
            var returnUrl = _configuration["PayOS:ReturnUrl"];
            var cancelUrl = _configuration["PayOS:CancelUrl"];
            var clientId = _configuration["PayOS:ClientId"];
            var checksumKey = _configuration["PayOS:ChecksumKey"] ?? string.Empty;
            string signature = Helper.GenerateChecksum(model.Amount.ToString(), cancelUrl, "thanh toan", model.BookingId.ToString(), returnUrl, checksumKey);


            var paymentData = new PaymentData(
                orderCode: model.BookingId,
                amount: (int)model.Amount,
                description: "Thanh toan",
                items: new List<ItemData>(),
                cancelUrl: cancelUrl,
                returnUrl: returnUrl
            );

            
            var result = await _payOS.createPaymentLink(paymentData);

            return Ok(new { PaymentUrl = result.checkoutUrl });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                var webhookData = _payOS.verifyPaymentWebhookData(webhookBody);

                if (_bookingService.UpdateBookingStatus((int)webhookData.orderCode, Constants.BookingStatus.Paid))
                {
                    _bookingService.AddTransactionLogAndRevenueTransaction((int)webhookData.orderCode);
                    return Ok();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Webhook: {ex.Message}");
                return BadRequest(ex);
            }
        }
    }
}
