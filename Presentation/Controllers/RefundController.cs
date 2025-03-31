using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
                return NotFound(new { message = "Không có refund nào được tìm thấy" });
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
                return NotFound(new { message = $"Không tìm thấy refund với ID {id}" });
            }
            return Ok(refund);
        }

        // POST: api/Refund/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestModel refundRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var refundResponse = await _refundRepository.CreateRefundRequestAsync(refundRequest);
                return Ok(new
                {
                    message = "Refund request submitted successfully",
                    refundId = refundResponse.RefundId,
                    refundAmount = refundResponse.RefundAmount,
                    timeRemaining = refundResponse.TimeRemaining,
                    refundPercentage = refundResponse.RefundPercentage,
                    bank = refundResponse.Bank
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Refund/preview/{bookingId}
        [HttpGet("preview/{bookingId}")]
        public async Task<IActionResult> PreviewRefund(int bookingId)
        {
            try
            {
                var result = await _refundRepository.PreviewRefundRequestAsync(bookingId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }

        }


        // DELETE: api/Refund/5
        [HttpDelete("{refundId}")]
        public async Task<IActionResult> DeleteRefund(int refundId)
        {
            var refund = await _refundRepository.GetRefundByIdAsync(refundId);
            if (refund == null)
            {
                return NotFound(new { message = $"Không tìm thấy refund với ID {refundId}" });
            }

            await _refundRepository.DeleteRefundAsync(refundId);
            var result = await _refundRepository.SaveChangesAsync();

            if (result)
            {
                return Ok(new { message = "Refund request deleted successfully" });
            }

            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa refund" });
        }

        // PUT: api/Refund/complete/{refundId}
        [HttpPut("complete/{refundId}")]
        public async Task<IActionResult> CompleteRefund(int refundId)
        {
            try
            {
                var refund = await _refundRepository.GetRefundByIdAsync(refundId);
                if (refund == null)
                {
                    return NotFound(new { message = $"Không tìm thấy refund với ID {refundId}" });
                }

                if (refund.Status != "Processing")
                {
                    return BadRequest(new { message = "Chỉ có thể hoàn tất refund đang ở trạng thái Processing" });
                }

                refund.Status = "Completed"; // Cập nhật trạng thái
                await _refundRepository.UpdateRefundStatusAsync(refundId, "Completed");
                var result = await _refundRepository.SaveChangesAsync();

                if (result)
                {
                    return Ok(new { message = $"Refund với ID {refundId} đã được hoàn tất" });
                }

                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái refund" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}