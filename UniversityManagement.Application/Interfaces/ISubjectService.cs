using UniversityManagement.Application.DTOs.Subject;

namespace UniversityManagement.Application.Interfaces;

public interface ISubjectService
{
    Task<SubjectResponseDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
    Task<SubjectResponseDto> UpdateSubjectAsync(UpdateSubjectDto updateSubjectDto);
    Task<SubjectResponseDto?> GetSubjectByIdAsync(Guid id);
    Task<List<SubjectResponseDto>> GetAllSubjectsAsync();
    Task<List<SubjectResponseDto>> GetSubjectsByProgramIdAsync(Guid programId);
    Task<bool> DeleteSubjectAsync(Guid id);
}
