using CloudinaryDotNet;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly Cloudinary _cloudinary;

        public BookingController(IBookingService bookingService, IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _bookingService = bookingService;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            var result = await _bookingService.GetBookingHistory(userId);
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

        [HttpPost("report-issue")]
        public async Task<IActionResult> ReportBooking([FromForm] ReportRequest request)
        {
            if (request == null || request.UserId <= 0 || request.BookingId <= 0 || string.IsNullOrEmpty(request.Description))
                return BadRequest("Invalid Data");

            if (request.Image != null && request.Image.Length > 0)
            {
                using var stream = request.Image.OpenReadStream();

                var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
                {
                    File = new FileDescription(request.Image.FileName, stream),
                    Folder = "report",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    request.ImageUrl = uploadResult.SecureUrl.ToString();
                }
            }

            var result = await _bookingService.CreateBookingReport(request);
            if (result == 0)
            {
                return BadRequest("Booking has been already report");
            }
            return Ok(new
            {
                IsSuccess = true
            });
        }

        [HttpGet("payment-histor/{userId}")]
        public async Task<IActionResult> GetTransactionLog(int userId)
        {
            var result = await _bookingService.GetTransactionLog(userId);
            return Ok(result);
        }
        [HttpPost("recurring")]
        public async Task<IActionResult> CreateRecurringBookings(RecurringBookingRequest  request)
        {
            if (request == null)
                return BadRequest("Invalid Data");

            var bookingId = await _bookingService.CreateRecurringBookings(request);

            if (bookingId == 0)
            {
                return Ok(new { IsSuccess = false });
            }

            return Ok(new { IsSuccess = true, BookingId = bookingId });
        }
    }
}
