using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IEmailSender _emailSender;

        public RefundController(IRefundRepository refundRepository, IEmailSender emailSender)
        {
            _refundRepository = refundRepository;
            _emailSender = emailSender;
        }

        // GET: api/Refund
        [HttpGet]
        public async Task<IActionResult> GetAllRefunds([FromQuery] int? userId = null)
        {
            var refunds = await _refundRepository.GetAllRefundsAsync();
            if (userId.HasValue)
            {
                refunds = refunds.Where(r => r.UserId == userId.Value);
            }

            if (refunds == null || !refunds.Any())
            {
                return Ok(new RefundResponseModel
                {
                    Error = "NOT_FOUND",
                    Message = "Không có refund nào được tìm thấy"
                });
            }

            return Ok(refunds);
        }

        // GET: api/Refund/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRefundById(int id)
        {
            var refund = await _refundRepository.GetRefundByIdAsync(id);
            if (refund == null)
            {
                return Ok(new RefundResponseModel
                {
                    Error = "NOT_FOUND",
                    Message = $"Không tìm thấy refund với ID {id}"
                });
            }

            return Ok(refund);
        }

        // POST: api/Refund/request
        [HttpPost("request")]
        public async Task<IActionResult> CreateRefundRequest([FromBody] RefundRequestModel refundRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RefundResponseModel
                {
                    Error = "INVALID",
                    Message = "Dữ liệu đầu vào không hợp lệ"
                });
            }

            var response = await _refundRepository.CreateRefundRequestAsync(refundRequest);
            return Ok(response);
        }

        // GET: api/Refund/preview/{bookingId}
        [HttpGet("preview/{bookingId}")]
        public async Task<IActionResult> PreviewRefund(int bookingId)
        {
            var response = await _refundRepository.PreviewRefundRequestAsync(bookingId);
            return Ok(response);
        }

        // DELETE: api/Refund/{refundId}
        [HttpDelete("{refundId}")]
        public async Task<IActionResult> DeleteRefund(int refundId)
        {
            var refund = await _refundRepository.GetRefundByIdAsync(refundId);
            if (refund == null)
            {
                return Ok(new RefundResponseModel
                {
                    Error = "NOT_FOUND",
                    Message = $"Không tìm thấy refund với ID {refundId}"
                });
            }

            await _refundRepository.DeleteRefundAsync(refundId);
            var result = await _refundRepository.SaveChangesAsync();

            if (result)
            {
                return Ok(new RefundResponseModel
                {
                    Message = "Refund request deleted successfully",
                    Error = null
                });
            }

            return StatusCode(500, new RefundResponseModel
            {
                Error = "INTERNAL_ERROR",
                Message = "Đã xảy ra lỗi khi xóa refund"
            });
        }

        // PUT: api/Refund/complete/{refundId}
        [HttpPut("complete/{refundId}")]
        public async Task<IActionResult> CompleteRefund(int refundId)
        {
            var refund = await _refundRepository.GetRefundByIdAsync(refundId);
            if (refund == null)
            {
                return Ok(new RefundResponseModel
                {
                    Error = "NOT_FOUND",
                    Message = $"Không tìm thấy refund với ID {refundId}"
                });
            }
            if (refund.Status != "Processing")
            {
                return Ok(new RefundResponseModel
                {
                    Error = "INVALID",
                    Message = "Chỉ có thể hoàn tất refund đang ở trạng thái Processing"
                });
            }
            await _refundRepository.UpdateRefundStatusAsync(refundId, "Completed");
            var result = await _refundRepository.SaveChangesAsync();

            if (!result)
            {
                return StatusCode(500, new RefundResponseModel
                {
                    Error = "INTERNAL_ERROR",
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái refund"
                });
            }

            // Kiểm tra email trước khi gửi
            if (string.IsNullOrEmpty(refund.UserEmail))
            {
                return Ok(new RefundResponseModel
                {
                    Error = "WARNING",
                    Message = "Refund completed but could not send email notification (email not found)."
                });
            }

            try
            {
                var subject = "Thông báo hoàn tiền";
                var htmlMessage = $@"
                    <h2>Xin chào {refund.UserName ?? "Quý khách"},</h2>
                    <p>Chúng tôi từ DNSPort xin thông báo rằng khoản tiền {refund.RefundAmount:N0} VND đã được hoàn lại vào tài khoản ngân hàng {refund.BankAccountNumber} của bạn.</p>
                    <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                ";

                await _emailSender.SendEmailAsync(refund.UserEmail, subject, htmlMessage);

                return Ok(new RefundResponseModel
                {
                    Error = "SUCCESS",
                    Message = "Refund completed and notification sent."
                });
            }
            catch (Exception ex)
            {
                // Log lỗi gửi mail nhưng vẫn trả về thành công vì refund đã được hoàn tất
                return Ok(new RefundResponseModel
                {
                    Error = "WARNING",
                    Message = "Refund completed but failed to send email notification."
                });
            }
        }

        [HttpGet("all-refund")]
        public async Task<IActionResult> GetAllRefunds()
        {
            var refunds = await _refundRepository.GetAllRefundsAsync();
            return Ok(refunds);
        }
    }
}