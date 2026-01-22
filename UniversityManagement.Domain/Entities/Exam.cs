using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Exam : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    [Required]
    public Guid SubjectId { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    [MaxLength(200)]
    public string? Location { get; set; }

    [Range(typeof(decimal), "0", "100000")]
    public decimal MaxMarks { get; set; }

    // Navigation properties
    public Subject Subject { get; set; } = null!;
}

public enum ExamType
{
    Midterm = 1,
    Final = 2,
    Quiz = 3,
    Assignment = 4
}
