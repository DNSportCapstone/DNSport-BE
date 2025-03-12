using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoMapsController : ControllerBase
    {
        private readonly IGoMapsService _goMapsService;

        public GoMapsController(IGoMapsService goMapsService)
        {
            _goMapsService = goMapsService;
        }

        [HttpGet("get-coordinates")]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            var result = await _goMapsService.GetCoordinatesAsync(address);
            return Ok(result);
        }

        //Tính khoảng cách từ một điểm đến nhiều điểm khác
        [HttpGet("distance-matrix")]
        public async Task<IActionResult> GetDistanceMatrix([FromQuery] string origin, [FromQuery] string destinations)
        {
            var result = await _goMapsService.GetDistanceMatrixAsync(origin, destinations);
            return Ok(result);
        }

        //Tìm con đường gần nhất
        [HttpGet("nearest-road")]
        public async Task<IActionResult> GetNearestRoad([FromQuery] string points)
        {
            var result = await _goMapsService.GetNearestRoadAsync(points);
            return Ok(result);
        }

        [HttpGet("nearby-stadiums")]
        public async Task<IActionResult> GetNearbyStadiums([FromQuery] string userLocation)
        {
            var result = await _goMapsService.GetNearbyStadiumsAsync(userLocation);
            return Ok(result);
        }


    }
}
