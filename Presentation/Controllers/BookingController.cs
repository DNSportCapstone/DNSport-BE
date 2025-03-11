using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            var result = await _bookingRepository.GetBookingHistory(userId);
            return Ok(result);
        }
    }
}
