using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;

        public BookingController(IBookingRepository bookingRepository, IBookingService bookingService)
        {
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            var result = await _bookingRepository.GetBookingHistory(userId);
            return Ok(result);
        }
        [HttpPost("multiple")]
        public async Task<IActionResult> CreateMultipleBookings([FromBody] MultipleBookingsRequest request)
        {
            if (request == null || request.Fields == null || !request.Fields.Any())
                return BadRequest("Invalid Data");

            var bookingId = await _bookingService.CreateMultipleBookings(request);

            if (bookingId == 0)
            {
                return Ok(new { IsSuccess = false });
            }

            return Ok(new { IsSuccess = true, BookingId = bookingId });
        }
    }
}
