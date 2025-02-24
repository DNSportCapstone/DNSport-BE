using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;

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


        [HttpPut("update/{ratingId}")]
        public async Task<IActionResult> UpdateRating(int ratingId, [FromBody] RatingModel model)
        {
            try
            {
                var result = await _ratingRepository.UpdateRatingAsync(ratingId, model);
                return Ok(new { Success = result, Message = "Rating updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRatings(int userId)
        {
            var ratings = await _ratingRepository.GetRatingsAsync(userId);
            return Ok(ratings);
        }
    }
}
