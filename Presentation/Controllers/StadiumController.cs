using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.Model;
using DataAccess.Repositories.Implement;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IStadiumRepository _stadiumRepository;
        private readonly IStadiumService _stadiumService;
        private readonly Cloudinary _cloudinary;

        public StadiumController(
            IFieldRepository fieldRepository,
            IStadiumRepository stadiumRepository,
            IStadiumService stadiumService,
            IOptions<CloudinarySettings> config)
        {
            _fieldRepository = fieldRepository;
            _stadiumRepository = stadiumRepository;
            _stadiumService = stadiumService;
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        public async Task<IActionResult> GetStadiumData()
        {
            var result = await _stadiumRepository.GetStadiumData();
            return Ok(result);
        }

        [HttpGet("search-by-name/{stadiumName}")]
        public async Task<IActionResult> GetStadiumByName(string stadiumName)
        {
            var result = await _stadiumService.GetStadiumByName(stadiumName);
            return Ok(result);
        }

        [HttpPost("Stadium")]
        public async Task<IActionResult> AddStadium([FromBody] StadiumRequestModel model)
        {
            var result = await _stadiumRepository.AddStadium(model);
            return Ok(result);
        }

        [HttpGet("Field")]
        public async Task<IActionResult> GetFieldHomeData()
        {
            var result = await _fieldRepository.GetFieldHomeData();
            return Ok(result);
        }

        [HttpGet("lessor/{userId}")]
        public async Task<IActionResult> GetStadiumsByLessorId(int userId)
        {
            try
            {
                var result = await _stadiumRepository.GetStadiumsByLessorIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetStadiumsByUserId(int userId)
        {
            var stadiums = await _stadiumRepository.GetStadiumsByUserId(userId);
            if (stadiums == null || !stadiums.Any())
                return NotFound("No stadiums found for this user.");

            return Ok(stadiums);
        }
    }
}
