using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Infrastructure.Repositories;

public class AnnouncementRepository
    : Repository<Announcement>, IAnnouncementRepository
{
    public Task<List<Announcement>> GetActiveAnnouncementsAsync(
        Guid? programId,
        Guid? subjectId)
    {
        var query = _entities.Values.Where(a =>
            a.IsActive &&
            (a.ExpiryDate == null || a.ExpiryDate > DateTime.UtcNow));

        if (programId.HasValue)
        {
            query = query.Where(a =>
                a.TargetAudience == TargetAudience.All ||
                (a.TargetAudience == TargetAudience.Program &&
                 a.ProgramId == programId));
        }

        if (subjectId.HasValue)
        {
            query = query.Where(a =>
                a.TargetAudience == TargetAudience.All ||
                (a.TargetAudience == TargetAudience.Subject &&
                 a.SubjectId == subjectId));
        }

        return Task.FromResult(
            query.OrderByDescending(a => a.PublishDate).ToList());
    }

    public Task<List<Announcement>> GetByTeacherIdAsync(Guid teacherId)
    {
        var announcements = _entities.Values
            .Where(a => a.TeacherId == teacherId)
            .OrderByDescending(a => a.PublishDate)
            .ToList();

        return Task.FromResult(announcements);
    }
}
