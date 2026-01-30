using UniversityManagement.Application.DTOs.Announcement;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _repo;

        public AnnouncementService(IAnnouncementRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AnnouncementResponseDto>> GetAllAnnouncementsAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToResponse).ToList();
        }

        public async Task<AnnouncementResponseDto?> GetAnnouncementByIdAsync(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);

            return a == null ? null : MapToResponse(a);
        }

        public async Task<List<AnnouncementResponseDto>> GetAnnouncementsByTeacherAsync(Guid teacherId)
        {
            var list = await _repo.GetByTeacherIdAsync(teacherId);
            return list.Select(MapToResponse).ToList();
        }

        // ✅ REQUIRED BY YOUR INTERFACE
        public async Task<List<AnnouncementResponseDto>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId)
        {
            var list = await _repo.GetActiveAnnouncementsAsync(programId, subjectId);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<AnnouncementResponseDto> CreateAnnouncementAsync(CreateAnnouncementDto dto)
        {
            // Protect against null strings -> avoids "possible null reference assignment"
            var title = dto.Title ?? string.Empty;
            var content = dto.Content ?? string.Empty;

            var entity = new Announcement
            {
                Title = title,
                Content = content,

                TeacherId = dto.TeacherId,
                ProgramId = dto.ProgramId,
                SubjectId = dto.SubjectId,

                TargetAudience = string.IsNullOrWhiteSpace(dto.TargetAudience) ? "All" : dto.TargetAudience,
                ExpiryDate = dto.ExpiryDate,

                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var created = await _repo.AddAsync(entity);
            return MapToResponse(created);
        }

        // ✅ Update returns non-nullable response when the entity exists.
        public async Task<AnnouncementResponseDto> UpdateAnnouncementAsync(UpdateAnnouncementDto updateAnnouncementDto)
        {
            var existing = await _repo.GetByIdAsync(updateAnnouncementDto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Announcement not found.");

            // Only overwrite if incoming values are not null/empty
            if (!string.IsNullOrWhiteSpace(updateAnnouncementDto.Title))
                existing.Title = updateAnnouncementDto.Title;

            if (!string.IsNullOrWhiteSpace(updateAnnouncementDto.Content))
                existing.Content = updateAnnouncementDto.Content;

            if (!string.IsNullOrWhiteSpace(updateAnnouncementDto.TargetAudience))
                existing.TargetAudience = updateAnnouncementDto.TargetAudience;

            // If these are nullable GUIDs in your DTO, only assign when provided
            if (updateAnnouncementDto.ProgramId.HasValue)
                existing.ProgramId = updateAnnouncementDto.ProgramId;

            if (updateAnnouncementDto.SubjectId.HasValue)
                existing.SubjectId = updateAnnouncementDto.SubjectId;

            // ExpiryDate may be nullable -> assign directly
            existing.ExpiryDate = updateAnnouncementDto.ExpiryDate;

            existing.IsActive = updateAnnouncementDto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(existing);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAnnouncementAsync(Guid id)
        {
            return await _repo.DeleteAsync(id);
        }

        private static AnnouncementResponseDto MapToResponse(Announcement a)
        {
            return new AnnouncementResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,

                // PublishDate not in entity -> use CreatedAt
                PublishDate = a.CreatedAt,

                TeacherId = a.TeacherId,
                TeacherName = a.Teacher == null ? null : $"{a.Teacher.FirstName} {a.Teacher.LastName}",

                TargetAudience = a.TargetAudience,
                ProgramId = a.ProgramId,
                ProgramName = a.Program?.Name,

                SubjectId = a.SubjectId,
                SubjectName = a.Subject?.Name,

                ExpiryDate = a.ExpiryDate,
                IsActive = a.IsActive
            };
        }
    }
}
