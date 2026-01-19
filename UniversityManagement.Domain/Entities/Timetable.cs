using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Timetable : BaseEntity
{
    public Guid ProgramId { get; set; }
    public Guid SubjectId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;

    // Navigation properties
    public Program Program { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
