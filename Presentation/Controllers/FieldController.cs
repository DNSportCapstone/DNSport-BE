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
        [HttpGet("stadium/{stadiumId}")]
        public async Task<IActionResult> GetFieldsByStadium(int stadiumId)
        {
            var fields = await _fieldService.GetFieldsByStadiumAsync(stadiumId);
            if (fields == null || fields.Count == 0)
            {
                return NotFound(new { message = "No fields found for this stadium." });
            }
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



    }

}
