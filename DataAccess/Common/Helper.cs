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
using System.Security.Cryptography;

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
        public static string GenerateHmacSHA256(string text, string key)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] messageBytes = encoding.GetBytes(text);
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }
        public static string GenerateChecksum(string amount, string cancelUrl, string description, string orderCode, string returnUrl, string checksumKey)
        {
            // Tạo chuỗi dữ liệu theo thứ tự đã yêu cầu
            string data = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

            // Mã hóa chuỗi với checksumKey bằng thuật toán HMAC_SHA256
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                // Trả về chữ ký dưới dạng hex string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
