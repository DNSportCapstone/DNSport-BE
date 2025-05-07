using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/vouchers")]
[ApiController]
//[Authorize(Roles = "Lessor")] // Chỉ cho phép chủ sân quản lý voucher
public class VoucherController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VoucherController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// Lấy danh sách tất cả voucher
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<VoucherResponse>>> GetAllVouchers()
    {
        var vouchers = await _voucherService.GetAllVouchersAsync();
        return Ok(vouchers);
    }
    [HttpPost("create")]
    public async Task<ActionResult<CreateVoucherResponse>> CreateVoucher([FromBody] CreateVoucherRequest request)
    {
        try
        {
            var result = await _voucherService.CreateVoucherAsync(request);
            return CreatedAtAction(nameof(GetVoucherById), new { id = result.VoucherId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<VoucherResponse>> GetVoucherById(int id)
    {
        var voucher = await _voucherService.GetVoucherByIdAsync(id);
        if (voucher == null)
            return NotFound(new { message = "Voucher not found" });

        return Ok(voucher);
    }
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateVoucher(int id, [FromBody] UpdateVoucherRequest request)
    {
        try
        {
            var result = await _voucherService.UpdateVoucherAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteVoucher(int id)
    {
        var success = await _voucherService.DeleteVoucherAsync(id);
        if (!success)
            return NotFound(new { message = "Voucher not found or already deleted" });

        return NoContent();
    }

    [HttpPost("apply")]
    public async Task<IActionResult> ApplyVoucher([FromBody] string? voucherCode)
    {
        var voucherResponse = await _voucherService.ApplyVoucher(voucherCode);
        return Ok(voucherResponse);
    }
}
