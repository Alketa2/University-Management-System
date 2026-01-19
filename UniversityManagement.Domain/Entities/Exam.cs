using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Exam : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    public Guid SubjectId { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Location { get; set; }
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
