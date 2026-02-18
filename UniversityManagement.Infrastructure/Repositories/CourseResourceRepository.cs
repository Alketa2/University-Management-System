using MongoDB.Driver;
using UniversityManagement.Domain.MongoEntities;
using UniversityManagement.Infrastructure.Data.Mongo;

namespace UniversityManagement.Infrastructure.Repositories;

public class CourseResourceRepository : ICourseResourceRepository
{
    private readonly MongoDbContext _context;

    public CourseResourceRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseResource>> GetBySubjectIdAsync(string subjectId)
    {
        return await _context.CourseResources
            .Find(r => r.SubjectId == subjectId)
            .ToListAsync();
    }

    public async Task CreateAsync(CourseResource resource)
    {
        await _context.CourseResources.InsertOneAsync(resource);
    }

    public async Task UpdateAsync(string id, CourseResource resource)
    {
        await _context.CourseResources.ReplaceOneAsync(r => r.Id == id, resource);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.CourseResources.DeleteOneAsync(r => r.Id == id);
    }

    public async Task<CourseResource?> GetByIdAsync(string id)
    {
        return await _context.CourseResources.Find(r => r.Id == id).FirstOrDefaultAsync();
    }
}
