using DataAccess.DTOs.Request;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }
        [HttpGet("get-all-fields")]
        public async Task<IActionResult> GetAllFields()
        {
            var fields = await _fieldService.GetAllFieldsAsync();
            return Ok(fields);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterField([FromBody] RegisterFieldRequest request)
        {
            var response = await _fieldService.RegisterFieldAsync(request);
            return Ok(response);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateField([FromBody] EditFieldRequest request)
        {
            var response = await _fieldService.EditFieldAsync(request);
            return Ok(response);
        }

        [HttpGet("fields-by-stadium-id/{stadiumId}")]
        public async Task<IActionResult> GetFieldsByStadiumId(int stadiumId)
        {
            var fields = await _fieldService.GetFieldsByStadiumId(stadiumId);
            return Ok(fields);
        }
        [HttpGet("{fieldId}")]
        public async Task<IActionResult> GetFieldById(int fieldId)
        {
            var field = await _fieldService.GetFieldByIdAsync(fieldId);
            if (field == null)
            {
                return NotFound("Field not found");
            }
            return Ok(field);
        }
        [HttpPut("set-status")]
        public async Task<IActionResult> SetFieldStatus([FromBody] FieldStatusRequest request)
        {
            if (request == null || request.FieldId <= 0 || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest("Invalid request data.");
            }

            if (request.Status != "Active" && request.Status != "Disable")
            {
                return BadRequest("Status must be 'Active' or 'Disable'.");
            }

            var result = await _fieldService.SetFieldStatus(request);
            if (result == 0)
            {
                return NotFound("Field not found.");
            }

            return Ok(new
            {
                FieldId = request.FieldId,
                Status = request.Status,
                Message = $"Field status updated to {request.Status} successfully."
            });
        }
        }

}
