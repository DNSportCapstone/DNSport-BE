using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Services.Interfaces;
using DataAccess.DTOs.Request;
using DataAccess.Repositories.Interfaces;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IUserRepository userRepository, IConfiguration configuration, IUserDetailRepository userDetailRepository, IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var response = await _authService.GoogleLogin(request);

            if (string.IsNullOrEmpty(response.AccessToken))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public IActionResult GetRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = _authService.GetRefreshToken(request);

            if (string.IsNullOrEmpty(response.RefreshToken))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("access-token")]
        [AllowAnonymous]
        public IActionResult GetAccessToken([FromBody] AccessTokenRequest request)
        {
            var response = _authService.GetAccessToken(request);

            if (string.IsNullOrEmpty(response.AccessToken))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
