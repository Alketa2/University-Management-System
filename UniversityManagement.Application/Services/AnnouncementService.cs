using UniversityManagement.Application.DTOs.Announcement;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IRepository<Teacher> _teacherRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly IRepository<Subject> _subjectRepository;

    public AnnouncementService(
        IAnnouncementRepository announcementRepository,
        IRepository<Teacher> teacherRepository,
        IRepository<Program> programRepository,
        IRepository<Subject> subjectRepository)
    {
        _announcementRepository = announcementRepository;
        _teacherRepository = teacherRepository;
        _programRepository = programRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<AnnouncementResponseDto> CreateAnnouncementAsync(CreateAnnouncementDto createAnnouncementDto)
    {
        var announcement = new Announcement
        {
            Title = createAnnouncementDto.Title,
            Content = createAnnouncementDto.Content,
            TeacherId = createAnnouncementDto.TeacherId,
            TargetAudience = ParseTargetAudience(createAnnouncementDto.TargetAudience),
            ProgramId = createAnnouncementDto.ProgramId,
            SubjectId = createAnnouncementDto.SubjectId,
            PublishDate = DateTime.UtcNow,
            ExpiryDate = createAnnouncementDto.ExpiryDate,
            IsActive = true
        };

        var createdAnnouncement = await _announcementRepository.AddAsync(announcement);
        return await MapToResponseDto(createdAnnouncement);
    }

    public async Task<AnnouncementResponseDto> UpdateAnnouncementAsync(UpdateAnnouncementDto updateAnnouncementDto)
    {
        var announcement = await _announcementRepository.GetByIdAsync(updateAnnouncementDto.Id);
        if (announcement == null)
            throw new KeyNotFoundException($"Announcement with ID {updateAnnouncementDto.Id} not found.");

        announcement.Title = updateAnnouncementDto.Title;
        announcement.Content = updateAnnouncementDto.Content;
        announcement.TargetAudience = ParseTargetAudience(updateAnnouncementDto.TargetAudience);
        announcement.ProgramId = updateAnnouncementDto.ProgramId;
        announcement.SubjectId = updateAnnouncementDto.SubjectId;
        announcement.ExpiryDate = updateAnnouncementDto.ExpiryDate;
        announcement.IsActive = updateAnnouncementDto.IsActive;

        var updatedAnnouncement = await _announcementRepository.UpdateAsync(announcement);
        return await MapToResponseDto(updatedAnnouncement);
    }

    public async Task<AnnouncementResponseDto?> GetAnnouncementByIdAsync(Guid id)
    {
        var announcement = await _announcementRepository.GetByIdAsync(id);
        return announcement == null ? null : await MapToResponseDto(announcement);
    }

    public async Task<List<AnnouncementResponseDto>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId)
    {
        var announcements = await _announcementRepository.GetActiveAnnouncementsAsync(programId, subjectId);
        var result = new List<AnnouncementResponseDto>();
        foreach (var announcement in announcements)
        {
            result.Add(await MapToResponseDto(announcement));
        }
        return result;
    }

    public async Task<List<AnnouncementResponseDto>> GetAnnouncementsByTeacherAsync(Guid teacherId)
    {
        var announcements = await _announcementRepository.GetByTeacherIdAsync(teacherId);
        var result = new List<AnnouncementResponseDto>();
        foreach (var announcement in announcements)
        {
            result.Add(await MapToResponseDto(announcement));
        }
        return result;
    }

    public async Task<bool> DeleteAnnouncementAsync(Guid id)
    {
        return await _announcementRepository.DeleteAsync(id);
    }

    private static TargetAudience ParseTargetAudience(string targetAudience)
    {
        return Enum.TryParse<TargetAudience>(targetAudience, true, out var result) ? result : TargetAudience.All;
    }

    private async Task<AnnouncementResponseDto> MapToResponseDto(Announcement announcement)
    {
        var teacher = await _teacherRepository.GetByIdAsync(announcement.TeacherId);
        var program = announcement.ProgramId.HasValue ? await _programRepository.GetByIdAsync(announcement.ProgramId.Value) : null;
        var subject = announcement.SubjectId.HasValue ? await _subjectRepository.GetByIdAsync(announcement.SubjectId.Value) : null;

        return new AnnouncementResponseDto
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            TeacherId = announcement.TeacherId,
            TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : string.Empty,
            TargetAudience = announcement.TargetAudience.ToString(),
            ProgramId = announcement.ProgramId,
            ProgramName = program?.Name,
            SubjectId = announcement.SubjectId,
            SubjectName = subject?.Name,
            PublishDate = announcement.PublishDate,
            ExpiryDate = announcement.ExpiryDate,
            IsActive = announcement.IsActive,
            CreatedAt = announcement.CreatedAt,
            UpdatedAt = announcement.UpdatedAt
        };
    }
}
