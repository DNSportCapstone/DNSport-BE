using DataAccess.DTOs;
using DataAccess.DTOs.Response;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueTransactionController : ControllerBase
    {
        private readonly IRevenueTransactionService _revenueTransactionService;

        public RevenueTransactionController(IRevenueTransactionService revenueTransactionService)
        {
            _revenueTransactionService = revenueTransactionService;
        }

        [HttpPost("owner-amount")]
        public async Task<ActionResult<List<OwnerAmountResponse>>> GetOwnerAmount([FromBody] OwnerAmountRequest request)
        {
            try
            {
                var result = await _revenueTransactionService.GetOwnerAmountByFieldIdAsync(request);
                if (result == null || !result.Any())
                {
                    return NotFound("No owner amount found for the specified field ID.");
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("owner-amount/{fieldId}")]
        public async Task<ActionResult<List<OwnerAmountResponse>>> GetOwnerAmountByFieldId(int fieldId)
        {
            try
            {
                var request = new OwnerAmountRequest { FieldId = fieldId };
                var result = await _revenueTransactionService.GetOwnerAmountByFieldIdAsync(request);
                if (result == null || !result.Any())
                {
                    return NotFound("No owner amount found for the specified field ID.");
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}