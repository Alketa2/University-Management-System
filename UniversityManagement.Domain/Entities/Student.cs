using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
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
