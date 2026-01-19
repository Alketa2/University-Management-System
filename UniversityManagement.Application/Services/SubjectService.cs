using UniversityManagement.Application.DTOs.Subject;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly IRepository<Teacher> _teacherRepository;

    public SubjectService(
        ISubjectRepository subjectRepository,
        IRepository<Program> programRepository,
        IRepository<Teacher> teacherRepository)
    {
        _subjectRepository = subjectRepository;
        _programRepository = programRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<SubjectResponseDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
    {
        var subject = new Subject
        {
            Name = createSubjectDto.Name,
            Code = createSubjectDto.Code,
            Description = createSubjectDto.Description,
            Credits = createSubjectDto.Credits,
            ProgramId = createSubjectDto.ProgramId,
            TeacherId = createSubjectDto.TeacherId,
            Semester = createSubjectDto.Semester,
            IsActive = true
        };

        var createdSubject = await _subjectRepository.AddAsync(subject);
        return await MapToResponseDto(createdSubject);
    }

    public async Task<SubjectResponseDto> UpdateSubjectAsync(UpdateSubjectDto updateSubjectDto)
    {
        var subject = await _subjectRepository.GetByIdAsync(updateSubjectDto.Id);
        if (subject == null)
            throw new KeyNotFoundException($"Subject with ID {updateSubjectDto.Id} not found.");

        subject.Name = updateSubjectDto.Name;
        subject.Code = updateSubjectDto.Code;
        subject.Description = updateSubjectDto.Description;
        subject.Credits = updateSubjectDto.Credits;
        subject.ProgramId = updateSubjectDto.ProgramId;
        subject.TeacherId = updateSubjectDto.TeacherId;
        subject.Semester = updateSubjectDto.Semester;
        subject.IsActive = updateSubjectDto.IsActive;

        var updatedSubject = await _subjectRepository.UpdateAsync(subject);
        return await MapToResponseDto(updatedSubject);
    }

    public async Task<SubjectResponseDto?> GetSubjectByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        return subject == null ? null : await MapToResponseDto(subject);
    }

    public async Task<List<SubjectResponseDto>> GetAllSubjectsAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();
        var result = new List<SubjectResponseDto>();
        foreach (var subject in subjects)
        {
            result.Add(await MapToResponseDto(subject));
        }
        return result;
    }

    public async Task<List<SubjectResponseDto>> GetSubjectsByProgramIdAsync(Guid programId)
    {
        var subjects = await _subjectRepository.GetByProgramIdAsync(programId);
        var result = new List<SubjectResponseDto>();
        foreach (var subject in subjects)
        {
            result.Add(await MapToResponseDto(subject));
        }
        return result;
    }

    public async Task<bool> DeleteSubjectAsync(Guid id)
    {
        return await _subjectRepository.DeleteAsync(id);
    }

    private async Task<SubjectResponseDto> MapToResponseDto(Subject subject)
    {
        var program = await _programRepository.GetByIdAsync(subject.ProgramId);
        var teacher = await _teacherRepository.GetByIdAsync(subject.TeacherId);

        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            Credits = subject.Credits,
            ProgramId = subject.ProgramId,
            ProgramName = program?.Name ?? string.Empty,
            TeacherId = subject.TeacherId,
            TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : string.Empty,
            Semester = subject.Semester,
            IsActive = subject.IsActive,
            CreatedAt = subject.CreatedAt,
            UpdatedAt = subject.UpdatedAt
        };
    }
}
