using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace UniversityManagement.Api.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string token, DateTime expiresAtUtc) CreateAccessToken(Guid userId, string email, string role)
        {
            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience");
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");

            var minutesStr = _config["Jwt:AccessTokenMinutes"] ?? "15";
            if (!int.TryParse(minutesStr, out var minutes)) minutes = 15;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (token, expires);
        }

        public (string token, DateTime expiresAtUtc) CreateRefreshToken()
        {
            var daysStr = _config["Jwt:RefreshTokenDays"] ?? "30";
            if (!int.TryParse(daysStr, out var days)) days = 30;

            var bytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(bytes);

            return (token, DateTime.UtcNow.AddDays(days));
        }
    }
}
