using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Student : BaseEntity
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Phone, MaxLength(30)]
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    [MaxLength(300)]
    public string? Address { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public StudentStatus Status { get; set; } = StudentStatus.Enrolled;

    // Navigation properties
    public ICollection<StudentProgram> StudentPrograms { get; set; } = new List<StudentProgram>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

public enum StudentStatus
{
    Enrolled = 1,
    Active = 2,
    Graduated = 3,
    Withdrawn = 4
}
