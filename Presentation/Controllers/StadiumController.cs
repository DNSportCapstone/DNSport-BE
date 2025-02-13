using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IStadium _stadiumService;
        public StadiumController(IFieldRepository fieldRepository, IStadium stadiumService)
        {
            _fieldRepository = fieldRepository;
            _stadiumService = stadiumService;
        }

        [HttpGet("Stadium")]
        public async Task<IActionResult> GetStadiumData()
        {
            var result = await _stadiumService.GetStadiumData();
            return Ok(result);
        }

        [HttpPost("Stadium")]
        public async Task<IActionResult> AddStadium([FromBody] StadiumRequestModel model)
        {
            var result = await _stadiumService.AddStadium(model);
            return Ok(result);
        }

        [HttpGet("Field")]
        public async Task<IActionResult> GetFieldHomeData()
        {
            var result = await _fieldRepository.GetFieldHomeData();
            return Ok(result);
        }
    }
}
