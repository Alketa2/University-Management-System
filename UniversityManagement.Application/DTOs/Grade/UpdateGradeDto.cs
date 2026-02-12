namespace UniversityManagement.Application.DTOs.Grade;

public class UpdateGradeDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? ExamId { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string? Comments { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
}
