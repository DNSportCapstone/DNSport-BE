using DataAccess.DTOs.Request;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        public UserController(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var result = await _userService.GetUserDetails(userId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUser(request);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userService.GetAllUser();
            return Ok(result);
        }


        //test send mail
        // how to use with a service
        // SendEmailAsync method with the following parameters:
        // string email, string subject, string htmlMessage
        [HttpGet("/mail")]
        public async Task<IActionResult> GetUserDetails([FromQuery] MailContent mailbody)
        {
            await _emailSender.SendEmailAsync(mailbody.To, mailbody.Subject, mailbody.Body);
            return Ok();
        }
    }
}
