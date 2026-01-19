using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public class TimetableRepository : Repository<Timetable>, ITimetableRepository
{
    public Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null)
    {
        var query = _entities.Values.Where(t => t.ProgramId == programId);
        
        if (semester.HasValue)
        {
            query = query.Where(t => t.Semester == semester.Value);
        }

        return Task.FromResult(query.ToList());
    }
}
