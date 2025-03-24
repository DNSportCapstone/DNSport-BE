using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IStadiumService _stadiumService;
        public AdminController(IBookingService bookingService, IUserService userService, IStadiumService stadiumService)
        {
            _bookingService = bookingService;
            _userService = userService;
            _stadiumService = stadiumService;
        }

        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReport()
        {
            var  result = await _bookingService.GetRevenueReport();
            return Ok(result);
        }

        [HttpGet("booking-report")]
        public async Task<IActionResult> GetBookingReport()
        {
            var result = await _bookingService.GetBookingReport();
            return Ok(result);
        }

        [HttpPost("disable-or-enable-stadium/{id}")]
        public async Task<IActionResult> DisableStadium(int id, [FromBody] string status)
        {
            var result = await _stadiumService.DisableStadium(id, status);
            return Ok(result);
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetUser()
        {
            var result = await _userService.GetAllUser();
            return Ok(result);
        }
    }
}
