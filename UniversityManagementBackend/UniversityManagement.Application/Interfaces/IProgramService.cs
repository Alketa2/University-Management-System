using UniversityManagement.Application.DTOs.Program;

namespace UniversityManagement.Application.Interfaces;

public interface IProgramService
{
    Task<ProgramResponseDto> CreateProgramAsync(CreateProgramDto createProgramDto);
    Task<ProgramResponseDto> UpdateProgramAsync(UpdateProgramDto updateProgramDto);
    Task<ProgramResponseDto?> GetProgramByIdAsync(Guid id);
    Task<List<ProgramResponseDto>> GetAllProgramsAsync();
    Task<bool> DeleteProgramAsync(Guid id);
}
