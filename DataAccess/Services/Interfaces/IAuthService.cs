using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
    public interface IAuthService
    {
        Task<GoogleLoginResponse> GoogleLogin(GoogleLoginRequest request);
        RefreshTokenResponse GetRefreshToken(RefreshTokenRequest request);
        AccessTokenResponse GetAccessToken(AccessTokenRequest request);
    }
}
