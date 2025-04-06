using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundRepository _refundRepository;

        public RefundController(IRefundRepository refundRepository)
        {
            _refundRepository = refundRepository;
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

            if (result)
            {
                return Ok(new RefundResponseModel
                {
                    Message = $"Refund với ID {refundId} đã được hoàn tất",
                    Error = null
                });
            }

            return StatusCode(500, new RefundResponseModel
            {
                Error = "INTERNAL_ERROR",
                Message = "Đã xảy ra lỗi khi cập nhật trạng thái refund"
            });
        }
    }
}