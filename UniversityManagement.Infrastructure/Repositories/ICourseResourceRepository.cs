using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Infrastructure.Repositories;

public interface ICourseResourceRepository
{
    Task<IEnumerable<CourseResource>> GetBySubjectIdAsync(string subjectId);
    Task CreateAsync(CourseResource resource);
    Task UpdateAsync(string id, CourseResource resource);
    Task DeleteAsync(string id);
    Task<CourseResource?> GetByIdAsync(string id);
}
