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

        [HttpPost("comment")]
        public async Task<IActionResult> AddOrUpdateComment([FromBody] RatingModel model)
        {
            try
            {
                var result = await _ratingRepository.AddOrUpdateCommentAsync(model);
                return Ok(new { Success = result, Message = "Comment updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

      
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRatingsByUser(int userId)
        {
            var ratings = await _ratingRepository.GetRatingsAsync(userId);
            return Ok(ratings);
        }
    }
}
