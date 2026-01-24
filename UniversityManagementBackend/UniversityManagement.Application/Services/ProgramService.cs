using UniversityManagement.Application.DTOs.Program;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IRepository<Program> _programRepository;

    public ProgramService(IRepository<Program> programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<ProgramResponseDto> CreateProgramAsync(CreateProgramDto createProgramDto)
    {
        var program = new Program
        {
            Name = createProgramDto.Name,
            Code = createProgramDto.Code,
            Description = createProgramDto.Description,
            Duration = createProgramDto.Duration,
            CreditsRequired = createProgramDto.CreditsRequired,
            StartDate = createProgramDto.StartDate,
            EndDate = createProgramDto.EndDate,
            IsActive = true
        };

        var createdProgram = await _programRepository.AddAsync(program);
        return MapToResponseDto(createdProgram);
    }

    public async Task<ProgramResponseDto> UpdateProgramAsync(UpdateProgramDto updateProgramDto)
    {
        var program = await _programRepository.GetByIdAsync(updateProgramDto.Id);
        if (program == null)
            throw new KeyNotFoundException($"Program with ID {updateProgramDto.Id} not found.");

        program.Name = updateProgramDto.Name;
        program.Code = updateProgramDto.Code;
        program.Description = updateProgramDto.Description;
        program.Duration = updateProgramDto.Duration;
        program.CreditsRequired = updateProgramDto.CreditsRequired;
        program.StartDate = updateProgramDto.StartDate;
        program.EndDate = updateProgramDto.EndDate;
        program.IsActive = updateProgramDto.IsActive;

        var updatedProgram = await _programRepository.UpdateAsync(program);
        return MapToResponseDto(updatedProgram);
    }

    public async Task<ProgramResponseDto?> GetProgramByIdAsync(Guid id)
    {
        var program = await _programRepository.GetByIdAsync(id);
        return program == null ? null : MapToResponseDto(program);
    }

    public async Task<List<ProgramResponseDto>> GetAllProgramsAsync()
    {
        var programs = await _programRepository.GetAllAsync();
        return programs.Select(MapToResponseDto).ToList();
    }

    public async Task<bool> DeleteProgramAsync(Guid id)
    {
        return await _programRepository.DeleteAsync(id);
    }

    private static ProgramResponseDto MapToResponseDto(Program program)
    {
        return new ProgramResponseDto
        {
            Id = program.Id,
            Name = program.Name,
            Code = program.Code,
            Description = program.Description,
            Duration = program.Duration,
            CreditsRequired = program.CreditsRequired,
            StartDate = program.StartDate,
            EndDate = program.EndDate,
            IsActive = program.IsActive,
            CreatedAt = program.CreatedAt,
            UpdatedAt = program.UpdatedAt
        };
    }
}
