using DataAccess.DTOs.Request;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IStadiumService _stadiumService;
        private readonly IFieldService _fieldService;
        private readonly IVoucherService _voucherService;

        public AdminController(IBookingService bookingService, IUserService userService, IStadiumService stadiumService, IFieldService fieldService, IVoucherService voucherService)
        {
            _bookingService = bookingService;
            _userService = userService;
            _stadiumService = stadiumService;
            _fieldService = fieldService;
            _voucherService = voucherService;
        }

        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReport()
        {
            var result = await _bookingService.GetRevenueReport();
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

        [HttpPost("set-user-status")]
        public async Task<IActionResult> SetUserStatus([FromBody] UserStatusRequest request)
        {
            var result = await _userService.SetUserStatus(request);
            return Ok(result);
        }

        [HttpGet("denounce-report")]
        public async Task<IActionResult> GetDenounceReport()
        {
            var result = await _bookingService.GetAllDenounce();
            return Ok(result);
        }

        [HttpPost("set-field-status")]
        public async Task<IActionResult> SetFieldStatus([FromBody] FieldStatusRequest request)
        {
            var result = await _fieldService.SetFieldStatus(request);
            return Ok(result);
        }

        [HttpPost("create-voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateOrUpdateVoucherRequest request)
        {
            var result = await _voucherService.CreateOrUpdateVoucher(request);
            return Ok(result);
        }

        [HttpGet("get-voucher")]
        public async Task<IActionResult> GetVoucher()
        {
            var result = await _voucherService.GetAllVouchers();
            return Ok(result);
        }

        [HttpPost("create-or-update-voucher")]
        public async Task<IActionResult> CreateOrUpdateVoucher([FromBody] CreateOrUpdateVoucherRequest request)
        {
            var result = await _voucherService.CreateOrUpdateVoucher(request);
            return Ok(result);
        }

        [HttpPost("set-user-role/{userId}")]
        public async Task<IActionResult> SetUserRole(int userId, [FromBody] int role)
        {
            var result = await _userService.SetUserRole(userId, role);
            return Ok(result);
        }

        [HttpPost("set-report-status/{reportId}")]
        public async Task<IActionResult> SetReportStatus(int reportId, [FromBody] string status)
        {
            var result = await _bookingService.SetReportStatus(reportId, status);
            return Ok(result);
        }

        [HttpPost("warning-lessor")]
        public async Task<IActionResult> WarningLessor([FromBody] WarningRequest request)
        {
            var result = await _userService.WarningLessor(request);
            return Ok(result);
        }

        [HttpGet("get-feild-report")]
        public async Task<IActionResult> GetFieldReportList()
        {
            var result = await _bookingService.GetFieldReportList();
            return Ok(result);
        }
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingStadiums()
        {
            var stadiums = await _stadiumService.GetPendingStadiumsAsync();
            return Ok(stadiums);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStadiumStatus([FromBody] UpdateStadiumStatusRequest request)
        {
            var result = await _stadiumService.UpdateStadiumStatusAsync(request);

            if (!result)
            {
                return BadRequest("Stadium not found or status is not pending.");
            }

            return Ok("Status updated successfully.");
        }

    }
}