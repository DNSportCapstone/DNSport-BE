using DataAccess.DTOs.Request;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Repositories.Interfaces;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IFieldRepository _fieldRepository;

        public FieldController(IFieldService fieldService, IFieldRepository fieldRepository)
        {
            _fieldService = fieldService;
            _fieldRepository = fieldRepository;
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

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveFields()
        {
            try
            {
                var fields = await _fieldRepository.GetActiveFields();
                return Ok(fields);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("active-fields-by-stadium-id/{stadiumId}")]
        public async Task<IActionResult> GetActiveFieldsByStadiumId(int stadiumId)
        {
            try
            {
                var fields = await _fieldRepository.GetActiveFieldsByStadiumId(stadiumId);
                return Ok(fields);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
