using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Teacher : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
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
