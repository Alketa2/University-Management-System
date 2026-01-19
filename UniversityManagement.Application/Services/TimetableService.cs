using UniversityManagement.Application.DTOs.Timetable;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class TimetableService : ITimetableService
{
    private readonly ITimetableRepository _timetableRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly IRepository<Subject> _subjectRepository;

    public TimetableService(
        ITimetableRepository timetableRepository,
        IRepository<Program> programRepository,
        IRepository<Subject> subjectRepository)
    {
        _timetableRepository = timetableRepository;
        _programRepository = programRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<TimetableResponseDto> CreateTimetableAsync(CreateTimetableDto createTimetableDto)
    {
        var timetable = new Timetable
        {
            ProgramId = createTimetableDto.ProgramId,
            SubjectId = createTimetableDto.SubjectId,
            DayOfWeek = (DayOfWeek)createTimetableDto.DayOfWeek,
            StartTime = createTimetableDto.StartTime,
            EndTime = createTimetableDto.EndTime,
            Room = createTimetableDto.Room,
            Semester = createTimetableDto.Semester,
            AcademicYear = createTimetableDto.AcademicYear
        };

        var createdTimetable = await _timetableRepository.AddAsync(timetable);
        return await MapToResponseDto(createdTimetable);
    }

    public async Task<TimetableResponseDto> UpdateTimetableAsync(UpdateTimetableDto updateTimetableDto)
    {
        var timetable = await _timetableRepository.GetByIdAsync(updateTimetableDto.Id);
        if (timetable == null)
            throw new KeyNotFoundException($"Timetable with ID {updateTimetableDto.Id} not found.");

        timetable.ProgramId = updateTimetableDto.ProgramId;
        timetable.SubjectId = updateTimetableDto.SubjectId;
        timetable.DayOfWeek = (DayOfWeek)updateTimetableDto.DayOfWeek;
        timetable.StartTime = updateTimetableDto.StartTime;
        timetable.EndTime = updateTimetableDto.EndTime;
        timetable.Room = updateTimetableDto.Room;
        timetable.Semester = updateTimetableDto.Semester;
        timetable.AcademicYear = updateTimetableDto.AcademicYear;

        var updatedTimetable = await _timetableRepository.UpdateAsync(timetable);
        return await MapToResponseDto(updatedTimetable);
    }

    public async Task<TimetableResponseDto?> GetTimetableByIdAsync(Guid id)
    {
        var timetable = await _timetableRepository.GetByIdAsync(id);
        return timetable == null ? null : await MapToResponseDto(timetable);
    }

    public async Task<List<TimetableResponseDto>> GetTimetableByProgramAsync(Guid programId, string? semester)
    {
        int? sem = null;
        if (!string.IsNullOrEmpty(semester) && int.TryParse(semester, out var semValue))
        {
            sem = semValue;
        }

        var timetables = await _timetableRepository.GetByProgramIdAsync(programId, sem);
        var result = new List<TimetableResponseDto>();
        foreach (var timetable in timetables)
        {
            result.Add(await MapToResponseDto(timetable));
        }
        return result;
    }

    public async Task<bool> DeleteTimetableAsync(Guid id)
    {
        return await _timetableRepository.DeleteAsync(id);
    }

    private async Task<TimetableResponseDto> MapToResponseDto(Timetable timetable)
    {
        var program = await _programRepository.GetByIdAsync(timetable.ProgramId);
        var subject = await _subjectRepository.GetByIdAsync(timetable.SubjectId);

        return new TimetableResponseDto
        {
            Id = timetable.Id,
            ProgramId = timetable.ProgramId,
            ProgramName = program?.Name ?? string.Empty,
            SubjectId = timetable.SubjectId,
            SubjectName = subject?.Name ?? string.Empty,
            DayOfWeek = (int)timetable.DayOfWeek,
            DayOfWeekName = timetable.DayOfWeek.ToString(),
            StartTime = timetable.StartTime,
            EndTime = timetable.EndTime,
            Room = timetable.Room,
            Semester = timetable.Semester,
            AcademicYear = timetable.AcademicYear,
            CreatedAt = timetable.CreatedAt,
            UpdatedAt = timetable.UpdatedAt
        };
    }
}
