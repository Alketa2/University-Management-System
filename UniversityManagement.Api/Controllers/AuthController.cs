using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using UniversityManagement.Api.Services;
using UniversityManagement.Application.DTOs.Auth;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UniversityDbContext _db;
        private readonly JwtTokenService _jwt;

        public AuthController(UniversityDbContext db, JwtTokenService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _db.Users.AnyAsync(u => u.Email == email))
                return BadRequest("Email already exists.");

            var user = new AppUser
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                PasswordHash = HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role.Trim()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(await IssueTokensAsync(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return Unauthorized("Invalid credentials.");
            if (!user.IsActive) return Unauthorized("User is inactive.");

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            return Ok(await IssueTokensAsync(user));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto dto)
        {
            var rt = await _db.RefreshTokens
                .Include(x => x.AppUser)
                .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);

            if (rt == null) return Unauthorized("Invalid refresh token.");
            if (rt.IsExpired) return Unauthorized("Refresh token expired.");
            if (rt.IsRevoked) return Unauthorized("Refresh token revoked.");

            // rotate refresh token
            rt.RevokedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(await IssueTokensAsync(rt.AppUser));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);
            if (rt == null) return Ok();

            rt.RevokedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok();
        }

        // ---------------- helpers ----------------

        private async Task<AuthResponseDto> IssueTokensAsync(AppUser user)
        {
            var (access, accessExp) = _jwt.CreateAccessToken(user.Id, user.Email, user.Role);
            var (refresh, refreshExp) = _jwt.CreateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                AppUserId = user.Id,
                Token = refresh,
                ExpiresAtUtc = refreshExp
            });

            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                AccessToken = access,
                AccessTokenExpiresAtUtc = accessExp,
                RefreshToken = refresh,
                RefreshTokenExpiresAtUtc = refreshExp
            };
        }

        private static string HashPassword(string password)
        {
            using var derive = new Rfc2898DeriveBytes(password, 16, 100_000, HashAlgorithmName.SHA256);
            var salt = derive.Salt;
            var key = derive.GetBytes(32);
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(key);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var parts = hash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedKey = Convert.FromBase64String(parts[1]);

            using var derive = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var key = derive.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(key, storedKey);
        }
    }
}
