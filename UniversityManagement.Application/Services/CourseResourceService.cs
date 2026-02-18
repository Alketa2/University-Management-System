using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.MongoEntities;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class CourseResourceService : ICourseResourceService
{
    private readonly ICourseResourceRepository _repository;

    public CourseResourceService(ICourseResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CourseResource>> GetSubjectResourcesAsync(string subjectId)
    {
        return await _repository.GetBySubjectIdAsync(subjectId);
    }

    public async Task<CourseResource> AddResourceAsync(CourseResource resource)
    {
        await _repository.CreateAsync(resource);
        return resource;
    }

    public async Task UpdateResourceAsync(string resourceId, CourseResource resource)
    {
        await _repository.UpdateAsync(resourceId, resource);
    }

    public async Task DeleteResourceAsync(string resourceId)
    {
        await _repository.DeleteAsync(resourceId);
    }
}
