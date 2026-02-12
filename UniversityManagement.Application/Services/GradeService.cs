using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.DTOs.Grade;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Application.Services;

public class GradeService : IGradeService
{
    private readonly UniversityDbContext _context;

    public GradeService(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<GradeResponseDto> CreateGradeAsync(CreateGradeDto createGradeDto)
    {
        var grade = new Grade
        {
            StudentId = createGradeDto.StudentId,
            SubjectId = createGradeDto.SubjectId,
            ExamId = createGradeDto.ExamId,
            Score = createGradeDto.Score,
            MaxScore = createGradeDto.MaxScore,
            Comments = createGradeDto.Comments,
            GradedByTeacherId = createGradeDto.GradedByTeacherId,
            AcademicYear = createGradeDto.AcademicYear,
            Semester = createGradeDto.Semester
        };

        // Calculate percentage, letter grade, and grade point
        CalculateGradeMetrics(grade);

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return await MapToResponseDto(grade);
    }

    public async Task<GradeResponseDto> UpdateGradeAsync(UpdateGradeDto updateGradeDto)
    {
        var grade = await _context.Grades.FindAsync(updateGradeDto.Id)
            ?? throw new KeyNotFoundException($"Grade with ID {updateGradeDto.Id} not found");

        grade.StudentId = updateGradeDto.StudentId;
        grade.SubjectId = updateGradeDto.SubjectId;
        grade.ExamId = updateGradeDto.ExamId;
        grade.Score = updateGradeDto.Score;
        grade.MaxScore = updateGradeDto.MaxScore;
        grade.Comments = updateGradeDto.Comments;
        grade.GradedByTeacherId = updateGradeDto.GradedByTeacherId;
        grade.AcademicYear = updateGradeDto.AcademicYear;
        grade.Semester = updateGradeDto.Semester;

        // Recalculate metrics
        CalculateGradeMetrics(grade);

        await _context.SaveChangesAsync();
        return await MapToResponseDto(grade);
    }

    public async Task<GradeResponseDto?> GetGradeByIdAsync(Guid id)
    {
        var grade = await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .Include(g => g.Exam)
            .Include(g => g.GradedByTeacher)
            .FirstOrDefaultAsync(g => g.Id == id);

        return grade == null ? null : await MapToResponseDto(grade);
    }

    public async Task<List<GradeResponseDto>> GetGradesByStudentAsync(Guid studentId)
    {
        var grades = await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .Include(g => g.Exam)
            .Include(g => g.GradedByTeacher)
            .Where(g => g.StudentId == studentId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        var tasks = grades.Select(MapToResponseDto);
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<List<GradeResponseDto>> GetGradesBySubjectAsync(Guid subjectId)
    {
        var grades = await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .Include(g => g.Exam)
            .Include(g => g.GradedByTeacher)
            .Where(g => g.SubjectId == subjectId)
            .OrderBy(g => g.Student.LastName)
            .ThenBy(g => g.Student.FirstName)
            .ToListAsync();

        var tasks = grades.Select(MapToResponseDto);
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<List<GradeResponseDto>> GetGradesByExamAsync(Guid examId)
    {
        var grades = await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .Include(g => g.Exam)
            .Include(g => g.GradedByTeacher)
            .Where(g => g.ExamId == examId)
            .OrderBy(g => g.Student.LastName)
            .ThenBy(g => g.Student.FirstName)
            .ToListAsync();

        var tasks = grades.Select(MapToResponseDto);
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<StudentGPADto> GetStudentGPAAsync(Guid studentId, string? academicYear = null, int? semester = null)
    {
        var student = await _context.Students.FindAsync(studentId)
            ?? throw new KeyNotFoundException($"Student with ID {studentId} not found");

        var query = _context.Grades
            .Include(g => g.Subject)
            .Include(g => g.Exam)
            .Include(g => g.GradedByTeacher)
            .Where(g => g.StudentId == studentId);

        // Get semester grades if specified
        if (!string.IsNullOrEmpty(academicYear) && semester.HasValue)
        {
            query = query.Where(g => g.AcademicYear == academicYear && g.Semester == semester);
        }

        var grades = await query.ToListAsync();
        var gradeDtos = await Task.WhenAll(grades.Select(MapToResponseDto));

        // Calculate semester GPA
        var semesterGPA = grades.Any() 
            ? grades.Average(g => g.GradePoint) 
            : 0;

        // Calculate cumulative GPA (all grades)
        var allGrades = await _context.Grades
            .Where(g => g.StudentId == studentId)
            .ToListAsync();
        
        var cumulativeGPA = allGrades.Any() 
            ? allGrades.Average(g => g.GradePoint) 
            : 0;

        return new StudentGPADto
        {
            StudentId = studentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            SemesterGPA = Math.Round(semesterGPA, 2),
            CumulativeGPA = Math.Round(cumulativeGPA, 2),
            TotalCredits = grades.Count * 3, // Assuming 3 credits per course
            AcademicYear = academicYear ?? "",
            Semester = semester ?? 0,
            Grades = gradeDtos.ToList()
        };
    }

    public async Task<bool> DeleteGradeAsync(Guid id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) return false;

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper method to calculate percentage, letter grade, and grade point
    private void CalculateGradeMetrics(Grade grade)
    {
        // Calculate percentage
        grade.Percentage = grade.MaxScore > 0 
            ? Math.Round((grade.Score / grade.MaxScore) * 100, 2) 
            : 0;

        // Determine letter grade and grade point based on percentage
        (grade.LetterGrade, grade.GradePoint) = grade.Percentage switch
        {
            >= 90 => ("A", 4.0m),
            >= 80 => ("B", 3.0m),
            >= 70 => ("C", 2.0m),
            >= 60 => ("D", 1.0m),
            _ => ("F", 0.0m)
        };
    }

    private async Task<GradeResponseDto> MapToResponseDto(Grade grade)
    {
        // Load related entities if not already loaded
        if (grade.Student == null)
            await _context.Entry(grade).Reference(g => g.Student).LoadAsync();
        if (grade.Subject == null)
            await _context.Entry(grade).Reference(g => g.Subject).LoadAsync();
        if (grade.Exam == null && grade.ExamId.HasValue)
            await _context.Entry(grade).Reference(g => g.Exam).LoadAsync();
        if (grade.GradedByTeacher == null && grade.GradedByTeacherId.HasValue)
            await _context.Entry(grade).Reference(g => g.GradedByTeacher).LoadAsync();

        return new GradeResponseDto
        {
            Id = grade.Id,
            StudentId = grade.StudentId,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
            StudentEmail = grade.Student.Email,
            SubjectId = grade.SubjectId,
            SubjectName = grade.Subject.Name,
            SubjectCode = grade.Subject.Code,
            ExamId = grade.ExamId,
            ExamName = grade.Exam?.Name,
            Score = grade.Score,
            MaxScore = grade.MaxScore,
            LetterGrade = grade.LetterGrade,
            Percentage = grade.Percentage,
            GradePoint = grade.GradePoint,
            Comments = grade.Comments,
            GradedByTeacherId = grade.GradedByTeacherId,
            GradedByName = grade.GradedByTeacher != null 
                ? $"{grade.GradedByTeacher.FirstName} {grade.GradedByTeacher.LastName}" 
                : null,
            AcademicYear = grade.AcademicYear,
            Semester = grade.Semester,
            CreatedAt = grade.CreatedAt,
            UpdatedAt = grade.UpdatedAt
        };
    }
}
