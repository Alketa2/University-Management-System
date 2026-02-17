namespace UniversityManagement.Application.DTOs.Auth;

public class UpdateIdentityDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
