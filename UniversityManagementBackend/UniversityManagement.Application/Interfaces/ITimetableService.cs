using UniversityManagement.Application.DTOs.Timetable;

namespace UniversityManagement.Application.Interfaces;

public interface ITimetableService
{
    Task<TimetableResponseDto> CreateTimetableAsync(CreateTimetableDto createTimetableDto);
    Task<TimetableResponseDto> UpdateTimetableAsync(UpdateTimetableDto updateTimetableDto);
    Task<TimetableResponseDto?> GetTimetableByIdAsync(Guid id);
    Task<List<TimetableResponseDto>> GetTimetableByProgramAsync(Guid programId, string? semester);
    Task<bool> DeleteTimetableAsync(Guid id);
}
