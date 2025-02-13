using DataAccess.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Presentation
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldRepository _fieldRepository;
        public FieldController(IFieldRepository fieldRepository)
        {
            _fieldRepository = fieldRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetFieldHomeData()
        {
            var result = await _fieldRepository.GetFieldHomeData();
            return Ok(result);
        }
    }
}
