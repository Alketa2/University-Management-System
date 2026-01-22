using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Subject : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, 60)]
    public int Credits { get; set; }

    [Required]
    public Guid ProgramId { get; set; }

    [Required]
    public Guid TeacherId { get; set; }

    [Range(1, 40)]
    public int Semester { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Program Program { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
