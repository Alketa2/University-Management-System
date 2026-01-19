using UniversityManagement.Application.DTOs.Teacher;

namespace UniversityManagement.Application.Interfaces;

public interface ITeacherService
{
    Task<TeacherResponseDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto);
    Task<TeacherResponseDto> UpdateTeacherAsync(UpdateTeacherDto updateTeacherDto);
    Task<TeacherResponseDto?> GetTeacherByIdAsync(Guid id);
    Task<List<TeacherResponseDto>> GetAllTeachersAsync();
    Task<bool> DeleteTeacherAsync(Guid id);
}
