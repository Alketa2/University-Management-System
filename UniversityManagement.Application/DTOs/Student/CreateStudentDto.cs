using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Student;

public class CreateStudentDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Phone, MaxLength(30)]
    public string? Phone { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }
}
