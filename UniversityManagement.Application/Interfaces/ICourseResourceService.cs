using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Application.Interfaces;

public interface ICourseResourceService
{
    Task<IEnumerable<CourseResource>> GetSubjectResourcesAsync(string subjectId);
    Task<CourseResource> AddResourceAsync(CourseResource resource);
    Task UpdateResourceAsync(string resourceId, CourseResource resource);
    Task DeleteResourceAsync(string resourceId);
}
