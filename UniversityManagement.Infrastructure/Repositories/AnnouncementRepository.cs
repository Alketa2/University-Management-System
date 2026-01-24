using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class AnnouncementRepository : EfRepository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(UniversityDbContext db) : base(db) { }

    public async Task<List<Announcement>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId)
    {
        var now = DateTime.UtcNow;
        var query = _set.AsNoTracking()
            .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > now));

        if (programId.HasValue)
        {
            var pid = programId.Value;
            query = query.Where(a =>
                a.TargetAudience == TargetAudience.All ||
                (a.TargetAudience == TargetAudience.Program && a.ProgramId == pid));
        }

        if (subjectId.HasValue)
        {
            var sid = subjectId.Value;
            query = query.Where(a =>
                a.TargetAudience == TargetAudience.All ||
                (a.TargetAudience == TargetAudience.Subject && a.SubjectId == sid));
        }

        return await query
            .OrderByDescending(a => a.PublishDate)
            .ToListAsync();
    }

    public Task<List<Announcement>> GetByTeacherIdAsync(Guid teacherId)
        => _set.AsNoTracking()
            .Where(a => a.TeacherId == teacherId)
            .OrderByDescending(a => a.PublishDate)
            .ToListAsync();
}
