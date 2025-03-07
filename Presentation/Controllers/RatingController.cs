using DataAccess.Repositories.Interfaces;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        //Thêm đánh giá
        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingModel model)
        {
            try
            {
                var result = await _ratingRepository.AddRatingAsync(model);
                return Ok(new { Success = result, Message = "Rating added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //Chỉ cho phép reply 1 lần
        [HttpPost("reply")]
        public async Task<IActionResult> AddReply([FromBody] RatingReplyModel model)
        {
            try
            {
                var result = await _ratingRepository.AddReplyAsync(model);
                return Ok(new { Success = result, Message = "Reply added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //Kiểm tra xem người dùng đã rating chưa
        [HttpGet("check/{bookingId}/{userId}")]
        public async Task<IActionResult> CheckUserRating(int bookingId, int userId)
        {
            var rating = await _ratingRepository.GetRatingByBookingAsync(bookingId, userId);
            return Ok(new { HasRated = rating != null });
        }

        [HttpPost("detect/{ratingId}")]
        public async Task<IActionResult> DetectAndReportComment(int ratingId, [FromBody] string comment)
        {
            try
            {
                var isReported = await _ratingRepository.DetectAndReportCommentAsync(ratingId, comment);
                return Ok(new { Reported = isReported });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //Lấy danh sách comment vi phạm
        [HttpGet("reported-comments")]
        public async Task<IActionResult> GetReportedComments()
        {
            var reportedComments = await _ratingRepository.GetReportedCommentsAsync();
            return Ok(reportedComments);
        }

        //Lấy danh sách comment của một sân
        [HttpGet("field/{fieldId}/comments")]
        public async Task<IActionResult> GetCommentsByFieldId(int fieldId)
        {
            var comments = await _ratingRepository.GetCommentsByFieldIdAsync(fieldId);
            return Ok(comments);
        }

    }
}
