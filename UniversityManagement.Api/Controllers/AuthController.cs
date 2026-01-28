using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.DTOs.Auth;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UniversityDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher<AppUser> _hasher;

    public AuthController(
        UniversityDbContext db,
        IJwtTokenService jwt,
        IPasswordHasher<AppUser> hasher)
    {
        _db = db;
        _jwt = jwt;
        _hasher = hasher;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
        if (exists) return BadRequest("Email already registered.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Student" : dto.Role.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);

        // create refresh token
        var (rawRefresh, refreshHash, refreshExpires) = _jwt.CreateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = refreshExpires
        });

        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(user);

        return Created("", new AuthResponseDto
        {
            Email = user.Email,
            Role = user.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = rawRefresh,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user == null || !user.IsActive) return Unauthorized("Invalid email or password.");

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        // rotate refresh token every login
        var (rawRefresh, refreshHash, refreshExpires) = _jwt.CreateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = refreshExpires
        });
        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(user);

        return Ok(new AuthResponseDto
        {
            Email = user.Email,
            Role = user.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = rawRefresh,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto dto)
    {
        var incomingHash = _jwt.HashToken(dto.RefreshToken);

        var token = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash);

        if (token == null || token.User == null)
            return Unauthorized("Invalid refresh token.");

        if (!token.IsActive || !token.User.IsActive)
            return Unauthorized("Refresh token expired or revoked.");

        // rotate refresh token
        var (newRaw, newHash, newExpires) = _jwt.CreateRefreshToken();

        token.RevokedAtUtc = DateTime.UtcNow;
        token.ReplacedByTokenHash = newHash;

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = token.UserId,
            TokenHash = newHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = newExpires
        });

        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(token.User);

        return Ok(new AuthResponseDto
        {
            Email = token.User.Email,
            Role = token.User.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = newRaw,
            RefreshTokenExpiresAtUtc = newExpires
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequestDto dto)
    {
        // revoke only the provided refresh token
        var incomingHash = _jwt.HashToken(dto.RefreshToken);

        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash);
        if (token == null) return Ok(); // nothing to revoke

        token.RevokedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok("Logged out.");
    }
}
