using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Auth
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
