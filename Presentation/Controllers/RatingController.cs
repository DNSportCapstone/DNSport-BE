using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repositories.Implement;
using DataAccess.Repositories.Interfaces;

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

        //Lấy danh sách comment của một sân
        [HttpGet("field/{fieldId}/comments")]
        public async Task<IActionResult> GetCommentsByFieldId(int fieldId)
        {
            var comments = await _ratingRepository.GetCommentsByFieldIdAsync(fieldId);
            return Ok(comments);
        }

        [HttpGet("can-rate/{bookingId}/{userId}")]
        public async Task<IActionResult> CanRate(int bookingId, int userId)
        {
            try
            {
                var result = await _ratingRepository.CanRateAsync(bookingId, userId);
                return Ok(new
                {
                    Success = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
