using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Program : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(1, 40)]
    public int Duration { get; set; } // in semesters

    [Range(0, 500)]
    public int CreditsRequired { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<StudentProgram> StudentPrograms { get; set; } = new List<StudentProgram>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
