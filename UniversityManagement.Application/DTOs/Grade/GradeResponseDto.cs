namespace UniversityManagement.Application.DTOs.Grade;

public class GradeResponseDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public Guid? ExamId { get; set; }
    public string? ExamName { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public decimal GradePoint { get; set; }
    public string? Comments { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public string? GradedByName { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
