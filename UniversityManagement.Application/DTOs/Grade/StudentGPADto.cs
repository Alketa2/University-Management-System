namespace UniversityManagement.Application.DTOs.Grade;

public class StudentGPADto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal SemesterGPA { get; set; }
    public decimal CumulativeGPA { get; set; }
    public int TotalCredits { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public List<GradeResponseDto> Grades { get; set; } = new();
}
