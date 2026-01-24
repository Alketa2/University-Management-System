using UniversityManagement.Application.DTOs.Exam;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IRepository<Subject> _subjectRepository;

    public ExamService(
        IExamRepository examRepository,
        IRepository<Subject> subjectRepository)
    {
        _examRepository = examRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<ExamResponseDto> CreateExamAsync(CreateExamDto createExamDto)
    {
        var exam = new Exam
        {
            Name = createExamDto.Name,
            ExamType = ParseExamType(createExamDto.ExamType),
            SubjectId = createExamDto.SubjectId,
            ExamDate = createExamDto.ExamDate,
            StartTime = createExamDto.StartTime,
            EndTime = createExamDto.EndTime,
            Location = createExamDto.Location,
            MaxMarks = createExamDto.MaxMarks
        };

        var createdExam = await _examRepository.AddAsync(exam);
        return await MapToResponseDto(createdExam);
    }

    public async Task<ExamResponseDto> UpdateExamAsync(UpdateExamDto updateExamDto)
    {
        var exam = await _examRepository.GetByIdAsync(updateExamDto.Id);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {updateExamDto.Id} not found.");

        exam.Name = updateExamDto.Name;
        exam.ExamType = ParseExamType(updateExamDto.ExamType);
        exam.SubjectId = updateExamDto.SubjectId;
        exam.ExamDate = updateExamDto.ExamDate;
        exam.StartTime = updateExamDto.StartTime;
        exam.EndTime = updateExamDto.EndTime;
        exam.Location = updateExamDto.Location;
        exam.MaxMarks = updateExamDto.MaxMarks;

        var updatedExam = await _examRepository.UpdateAsync(exam);
        return await MapToResponseDto(updatedExam);
    }

    public async Task<ExamResponseDto?> GetExamByIdAsync(Guid id)
    {
        var exam = await _examRepository.GetByIdAsync(id);
        return exam == null ? null : await MapToResponseDto(exam);
    }

    public async Task<List<ExamResponseDto>> GetAllExamsAsync()
    {
        var exams = await _examRepository.GetAllAsync();
        var result = new List<ExamResponseDto>();
        foreach (var exam in exams)
        {
            result.Add(await MapToResponseDto(exam));
        }
        return result;
    }

    public async Task<List<ExamResponseDto>> GetExamsBySubjectAsync(Guid subjectId)
    {
        var exams = await _examRepository.GetBySubjectIdAsync(subjectId);
        var result = new List<ExamResponseDto>();
        foreach (var exam in exams)
        {
            result.Add(await MapToResponseDto(exam));
        }
        return result;
    }

    public async Task<bool> DeleteExamAsync(Guid id)
    {
        return await _examRepository.DeleteAsync(id);
    }

    private static ExamType ParseExamType(string examType)
    {
        return Enum.TryParse<ExamType>(examType, true, out var result) ? result : ExamType.Quiz;
    }

    private async Task<ExamResponseDto> MapToResponseDto(Exam exam)
    {
        var subject = await _subjectRepository.GetByIdAsync(exam.SubjectId);

        return new ExamResponseDto
        {
            Id = exam.Id,
            Name = exam.Name,
            ExamType = exam.ExamType.ToString(),
            SubjectId = exam.SubjectId,
            SubjectName = subject?.Name ?? string.Empty,
            ExamDate = exam.ExamDate,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            Location = exam.Location,
            MaxMarks = exam.MaxMarks,
            CreatedAt = exam.CreatedAt,
            UpdatedAt = exam.UpdatedAt
        };
    }
}
