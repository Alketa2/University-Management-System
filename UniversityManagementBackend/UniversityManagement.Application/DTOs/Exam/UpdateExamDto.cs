namespace UniversityManagement.Application.DTOs.Exam;

public class UpdateExamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Location { get; set; }
    public decimal MaxMarks { get; set; }
}
