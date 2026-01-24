using UniversityManagement.Application.DTOs.Announcement;

namespace UniversityManagement.Application.Interfaces;

public interface IAnnouncementService
{
    Task<AnnouncementResponseDto> CreateAnnouncementAsync(CreateAnnouncementDto createAnnouncementDto);
    Task<AnnouncementResponseDto> UpdateAnnouncementAsync(UpdateAnnouncementDto updateAnnouncementDto);
    Task<AnnouncementResponseDto?> GetAnnouncementByIdAsync(Guid id);
    Task<List<AnnouncementResponseDto>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId);
    Task<List<AnnouncementResponseDto>> GetAnnouncementsByTeacherAsync(Guid teacherId);
    Task<bool> DeleteAnnouncementAsync(Guid id);
}
