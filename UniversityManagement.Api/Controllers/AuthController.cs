using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Student" : dto.Role.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);

        // ✅ Automatically create a profile based on role
        if (user.Role == "Student")
        {
            _db.Students.Add(new Student
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow,
                Status = StudentStatus.Enrolled
            });
        }
        else if (user.Role == "Teacher")
        {
            _db.Teachers.Add(new Teacher
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow
            });
        }

        // create refresh token
        var (rawRefresh, refreshHash, refreshExpires) = _jwt.CreateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            AppUserId = user.Id,
            Token = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAtUtc = refreshExpires

        });

        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(user);

        var (studentId, teacherId, primaryProgramId) = await GetProfileInfoAsync(user.Id, user.Role);

        return Created("", new AuthResponseDto
        {
            UserId = user.Id,
            StudentId = studentId,
            TeacherId = teacherId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = rawRefresh,
            RefreshTokenExpiresAtUtc = refreshExpires,
            PrimaryProgramId = primaryProgramId
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
            AppUserId = user.Id,
            Token = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAtUtc = refreshExpires
        });
        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(user);

        var (studentId, teacherId, primaryProgramId) = await GetProfileInfoAsync(user.Id, user.Role);

        return Ok(new AuthResponseDto
        {
            UserId = user.Id,
            StudentId = studentId,
            TeacherId = teacherId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = rawRefresh,
            RefreshTokenExpiresAtUtc = refreshExpires,
            PrimaryProgramId = primaryProgramId
        });
    }

    [HttpPost("refresh")]
    
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto dto)
    {
        var incomingHash = _jwt.HashToken(dto.RefreshToken);

        var token = await _db.RefreshTokens
         .Include(rt => rt.AppUser)
         .FirstOrDefaultAsync(rt => rt.Token == incomingHash);


        if (token == null || token.AppUser == null)
            return Unauthorized("Invalid refresh token.");

        if (!token.AppUser.IsActive)
            return Unauthorized("User is inactive.");

        if (!token.IsActive)
        {
            var userTokens = await _db.RefreshTokens
                .Where(rt => rt.AppUserId == token.AppUserId && rt.RevokedAtUtc == null)
                .ToListAsync();

            foreach (var refreshToken in userTokens)
            {
                refreshToken.RevokedAtUtc = DateTime.UtcNow;
                refreshToken.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return Unauthorized("Refresh token expired or revoked.");
        }

        // rotate refresh token
        var (newRaw, newHash, newExpires) = _jwt.CreateRefreshToken();

        token.RevokedAtUtc = DateTime.UtcNow;
        token.UpdatedAt = DateTime.UtcNow;

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            AppUserId = token.AppUserId,
            Token = newHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAtUtc = newExpires
        });

        await _db.SaveChangesAsync();

        var (access, accessExpires) = _jwt.CreateAccessToken(token.AppUser);

        var (studentId, teacherId, primaryProgramId) = await GetProfileInfoAsync(token.AppUser.Id, token.AppUser.Role);

        return Ok(new AuthResponseDto
        {
            UserId = token.AppUser.Id,
            StudentId = studentId,
            TeacherId = teacherId,
            Email = token.AppUser.Email,
            FirstName = token.AppUser.FirstName,
            LastName = token.AppUser.LastName,
            Phone = token.AppUser.Phone,
            Address = token.AppUser.Address,
            Role = token.AppUser.Role,
            AccessToken = access,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = newRaw,
            RefreshTokenExpiresAtUtc = newExpires,
            PrimaryProgramId = primaryProgramId
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequestDto dto)
    {
        // revoke only the provided refresh token
        var incomingHash = _jwt.HashToken(dto.RefreshToken);

        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == incomingHash);
        if (token == null) return Ok(); // nothing to revoke

        token.RevokedAtUtc = DateTime.UtcNow;
        token.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok("Logged out.");
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateIdentityDto dto)
    {
        var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return NotFound("User not found");

        // Update core user
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.Address = dto.Address;
        user.UpdatedAt = DateTime.UtcNow;

        // Sync with Profile
        if (user.Role == "Student")
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student != null)
            {
                student.FirstName = dto.FirstName;
                student.LastName = dto.LastName;
                student.Email = dto.Email;
                student.Phone = dto.Phone;
                student.Address = dto.Address;
                student.UpdatedAt = DateTime.UtcNow;
            }
        }
        else if (user.Role == "Teacher")
        {
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Email == email);
            if (teacher != null)
            {
                teacher.FirstName = dto.FirstName;
                teacher.LastName = dto.LastName;
                teacher.Email = dto.Email;
                teacher.Phone = dto.Phone;
                teacher.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully" });
    }

    private async Task<(Guid? StudentId, Guid? TeacherId, Guid? PrimaryProgramId)> GetProfileInfoAsync(Guid userId, string role)
    {
        var email = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(email)) return (null, null, null);

        if (role == "Student")
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Email == email);
            return (student?.Id, null, student?.PrimaryProgramId);
        }

        if (role == "Teacher")
        {
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Email == email);
            return (null, teacher?.Id, null);
        }

        return (null, null, null);
    }
}
