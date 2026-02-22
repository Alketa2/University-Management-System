namespace UniversityManagement.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }

    public Guid UserId { get; set; }
    public Guid? StudentId { get; set; }
    public Guid? TeacherId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid? PrimaryProgramId { get; set; }
}
