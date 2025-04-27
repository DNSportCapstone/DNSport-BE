using DataAccess.Common;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IConfiguration _configuration;
        private readonly string _googleClientId;
        private readonly string _jwtSecret;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IUserDetailRepository userDetailRepository)
        {

            _userRepository = userRepository;
            _googleClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;
            _jwtSecret = configuration["JWT:Secret"] ?? string.Empty;
            _configuration = configuration;
            _userDetailRepository = userDetailRepository;
        }
        public async Task<GoogleLoginResponse> GoogleLogin(GoogleLoginRequest request)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);

                if (payload == null)
                {
                    return new GoogleLoginResponse
                    {
                        IsError = true,
                        Message = "Lỗi con mẹ mày rồi"
                    };
                }

                var userModel = await _userRepository.GetUserByEmail(payload.Email);
                if (string.IsNullOrEmpty(userModel.Email))
                {
                    userModel = new UserModel
                    {
                        Email = payload.Email,
                        RoleId = 3,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow
                    };
                    userModel = await _userRepository.AddNewUser(userModel);
                    var userDetailModel = new UserDetailModel
                    {
                        UserId = userModel.UserId,
                        FullName = payload.Name,
                        PhoneNumber = string.Empty,
                        Address = string.Empty
                    };
                    await _userDetailRepository.AddNewUserDetail(userDetailModel);
                    userModel.UserDetail = userDetailModel;
                }
                else if (userModel.Status != "Active")
                {
                    return new GoogleLoginResponse { Message = "User is not Active", Error = "Invalid User" };
                }

                var accessToken = Helper.GenerateJwtToken(userModel, 180, _jwtSecret, _configuration); // 30min

                return new GoogleLoginResponse { AccessToken = accessToken };
            }
            catch (Exception ex)
            {
                return new GoogleLoginResponse { Message = "Invalid Google token", Error = ex.Message };
            }
        }
        public RefreshTokenResponse GetRefreshToken(RefreshTokenRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new RefreshTokenResponse { Message = "Invalid Request" };
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(request.AccessToken);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var emailAddress = jwtToken.Claims.FirstOrDefault(c => c.Type == "emailAddress")?.Value;
                var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullName")?.Value;

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(roleId) && !string.IsNullOrEmpty(fullName))
                {
                    var userModel = new UserModel
                    {
                        UserId = int.Parse(userId),
                        Email = emailAddress,
                        RoleId = int.Parse(roleId),
                        UserDetail = new UserDetailModel
                        {
                            FullName = fullName
                        }
                    };

                    var refreshToken = Helper.GenerateJwtToken(userModel, 1440, _jwtSecret, _configuration); // 1 day

                    return new RefreshTokenResponse { RefreshToken = refreshToken };
                }
                else
                {
                    return new RefreshTokenResponse { Message = "Invalid Access token" };
                }
            }
            catch (Exception ex)
            {
                return new RefreshTokenResponse { Message = "Invalid Access token", Error = ex.Message };
            }
        }
        public AccessTokenResponse GetAccessToken(AccessTokenRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new AccessTokenResponse { Message = "Invalid Request" };
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(request.RefreshToken);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var emailAddress = jwtToken.Claims.FirstOrDefault(c => c.Type == "emailAddress")?.Value;
                var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullName")?.Value;

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(roleId) && !string.IsNullOrEmpty(fullName))
                {
                    var userModel = new UserModel
                    {
                        UserId = int.Parse(userId),
                        Email = emailAddress,
                        RoleId = int.Parse(roleId),
                        UserDetail = new UserDetailModel
                        {
                            FullName = fullName
                        }
                    };

                    var accessToken = Helper.GenerateJwtToken(userModel, 30, _jwtSecret, _configuration); // 30 Mins

                    return new AccessTokenResponse { AccessToken = accessToken };
                }
                else
                {
                    return new AccessTokenResponse { Message = "Invalid Access token" };
                }
            }
            catch (Exception ex)
            {
                return new AccessTokenResponse { Message = "Invalid Access token", Error = ex.Message };
            }
        }
    }
}
