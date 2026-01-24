using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class TimetableRepository : EfRepository<Timetable>, ITimetableRepository
{
    public TimetableRepository(UniversityDbContext db) : base(db) { }

    public async Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null)
    {
        var query = _set.AsNoTracking().Where(t => t.ProgramId == programId);

        if (semester.HasValue)
        {
            var sem = semester.Value;
            query = query.Where(t => t.Semester == sem);
        }

        return await query
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync();
    }
}
