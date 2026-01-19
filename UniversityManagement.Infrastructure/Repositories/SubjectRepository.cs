using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public Task<List<Subject>> GetByProgramIdAsync(Guid programId)
    {
        var subjects = _entities.Values
            .Where(s => s.ProgramId == programId)
            .ToList();
        return Task.FromResult(subjects);
    }
}
