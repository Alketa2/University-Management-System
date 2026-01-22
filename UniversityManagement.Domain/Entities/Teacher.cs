using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Teacher : BaseEntity
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Phone, MaxLength(30)]
    public string? Phone { get; set; }

    [Required, MaxLength(150)]
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;

    // Navigation properties
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}

public enum TeacherStatus
{
    Active = 1,
    Inactive = 2
}
