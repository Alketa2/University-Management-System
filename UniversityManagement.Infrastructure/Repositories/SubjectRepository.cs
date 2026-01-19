using UniversityManagement.Domain.Common;     
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


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
