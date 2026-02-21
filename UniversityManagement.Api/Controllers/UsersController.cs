using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.DTOs.Auth;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public UsersController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Check if email is already taken by another user
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);
            if (existingUser != null)
            {
                return BadRequest("Email is already in use by another account.");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Email.ToLower() == "admin@university.com") 
                return BadRequest("Cannot change role of the primary admin.");

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Email.ToLower() == "admin@university.com")
                return BadRequest("Cannot deactivate the primary admin.");

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { isActive = user.IsActive });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Email.ToLower() == "admin@university.com")
                return BadRequest("Cannot delete the primary admin.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
