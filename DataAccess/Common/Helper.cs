using DataAccess.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Common
{
    public static class Helper
    {
        public static string GenerateJwtToken(UserModel userModel, double expiredMinute, string jwtSecret, IConfiguration configuration)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userModel.Email ?? string.Empty),
                new Claim("userId", userModel.UserId.ToString() ?? string.Empty),
                new Claim("emailAddress", userModel.Email ?? string.Empty),
                new Claim("roleId", userModel.RoleId.ToString() ?? string.Empty),
                new Claim("fullName", userModel.UserDetail?.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(expiredMinute),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
