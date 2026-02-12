using UniversityManagement.Application.DTOs.Grade;

namespace UniversityManagement.Application.Interfaces;

public interface IGradeService
{
    Task<GradeResponseDto> CreateGradeAsync(CreateGradeDto createGradeDto);
    Task<GradeResponseDto> UpdateGradeAsync(UpdateGradeDto updateGradeDto);
    Task<GradeResponseDto?> GetGradeByIdAsync(Guid id);
    Task<List<GradeResponseDto>> GetGradesByStudentAsync(Guid studentId);
    Task<List<GradeResponseDto>> GetGradesBySubjectAsync(Guid subjectId);
    Task<List<GradeResponseDto>> GetGradesByExamAsync(Guid examId);
    Task<StudentGPADto> GetStudentGPAAsync(Guid studentId, string? academicYear = null, int? semester = null);
    Task<bool> DeleteGradeAsync(Guid id);
}
