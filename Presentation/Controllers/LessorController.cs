using DataAccess.Model;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessorController : ControllerBase
    {
        private readonly ILessorService _lessorService;
        public LessorController(ILessorService lessorService)
        {
            _lessorService = lessorService;
        }

        [AllowAnonymous]
        [HttpPost("create-lessor-contact")]
        public async Task<IActionResult> CreateLessorContact([FromBody] LessorContactModel lessorContact)
        {
            var result = await _lessorService.CreateLessorContact(lessorContact);
            return Ok(result);
        }

        [HttpGet("accept-lessor-contact/{id}")]
        public async Task<IActionResult> AcceptLessorContact(int id)
        {
            var result = await _lessorService.AcceptLessorContact(id);
            return Ok(result);
        }

        [HttpGet("get-all-lessor-contact")]
        public async Task<IActionResult> GetAllLessorContact()
        {
            var result = await _lessorService.GetAllLessorContact();
            return Ok(result);
        }

        [HttpGet("reject-lessor-contact/{id}")]
        public async Task<IActionResult> RejectLessorContact(int id)
        {
            var result = await _lessorService.RejectLessorContact(id);
            return Ok(result);
        }
    }
}
