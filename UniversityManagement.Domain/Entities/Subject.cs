using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public Guid ProgramId { get; set; }
    public Guid TeacherId { get; set; }
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
