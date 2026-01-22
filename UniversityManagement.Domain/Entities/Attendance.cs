using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Attendance : BaseEntity
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    Excused = 4
}
