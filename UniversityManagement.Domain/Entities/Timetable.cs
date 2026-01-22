using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Timetable : BaseEntity
{
    [Required]
    public Guid ProgramId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    [MaxLength(60)]
    public string? Room { get; set; }

    [Range(1, 40)]
    public int Semester { get; set; }

    [Required, MaxLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    // Navigation properties
    public Program Program { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
