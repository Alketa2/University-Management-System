using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Grade;

public class CreateGradeDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    public Guid? ExamId { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "Score must be a positive number")]
    public decimal Score { get; set; }

    [Required]
    [Range(0.01, 10000, ErrorMessage = "Max Score must be greater than 0")]
    public decimal MaxScore { get; set; } = 100;

    public string? Comments { get; set; }

    public Guid? GradedByTeacherId { get; set; }

    [Required]
    public string AcademicYear { get; set; } = string.Empty;

    [Required]
    [Range(1, 40, ErrorMessage = "Semester must be between 1 and 40")]
    public int Semester { get; set; }
}
