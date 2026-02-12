using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Grade : BaseEntity
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    public Guid? ExamId { get; set; } // Nullable - can be overall grade or exam-specific

    [Required]
    [Range(0, 1000)]
    public decimal Score { get; set; }

    [Required]
    [Range(1, 1000)]
    public decimal MaxScore { get; set; } = 100;

    [MaxLength(2)]
    public string LetterGrade { get; set; } = string.Empty; // A, B, C, D, F

    [Range(0, 100)]
    public decimal Percentage { get; set; }

    [Range(0, 4)]
    public decimal GradePoint { get; set; } // For GPA calculation (4.0 scale)

    [MaxLength(500)]
    public string? Comments { get; set; }

    public Guid? GradedByTeacherId { get; set; } // FK for GradedByTeacher - nullable for Admins

    [Required, MaxLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    [Required, Range(1, 40)]
    public int Semester { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Exam? Exam { get; set; }
    public Teacher? GradedByTeacher { get; set; } // Nullable
}
