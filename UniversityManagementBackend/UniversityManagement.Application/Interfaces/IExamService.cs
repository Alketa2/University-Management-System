using UniversityManagement.Application.DTOs.Exam;

namespace UniversityManagement.Application.Interfaces;

public interface IExamService
{
    Task<ExamResponseDto> CreateExamAsync(CreateExamDto createExamDto);
    Task<ExamResponseDto> UpdateExamAsync(UpdateExamDto updateExamDto);
    Task<ExamResponseDto?> GetExamByIdAsync(Guid id);
    Task<List<ExamResponseDto>> GetAllExamsAsync();
    Task<List<ExamResponseDto>> GetExamsBySubjectAsync(Guid subjectId);
    Task<bool> DeleteExamAsync(Guid id);
}
