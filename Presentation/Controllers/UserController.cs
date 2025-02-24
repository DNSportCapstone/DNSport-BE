using DataAccess.Implement;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly IEmailSender _emailSender;
        public UserController(IUser userService, IEmailSender emailSender)
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
        public async Task<IActionResult> UpdateUser([FromBody] UserModel model)
        {
            var result = await _userService.UpdateUser(model);
            return Ok(result);
        }



        //test send mail
        // how to use with a service
        // SendEmailAsync method with the following parameters:
        // string email, string subject, string htmlMessage
        [HttpGet("sendmail")]
        public async Task<IActionResult> GetUserDetails([FromQuery] MailContent mailbody)
        {
            await _emailSender.SendEmailAsync(mailbody.To, mailbody.Subject, mailbody.Body);
            return Ok();
        }
    }
}
