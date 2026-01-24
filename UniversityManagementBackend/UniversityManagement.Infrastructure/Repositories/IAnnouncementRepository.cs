using UniversityManagement.Domain.Common;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Infrastructure.Repositories;

public interface IAnnouncementRepository : IRepository<Announcement>
{
    Task<List<Announcement>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId);
    Task<List<Announcement>> GetByTeacherIdAsync(Guid teacherId);
}
