using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class AttendanceRepository : EfRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(UniversityDbContext db) : base(db) { }

    public async Task<List<Attendance>> GetByStudentIdAsync(
        Guid studentId,
        Guid? subjectId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _set.AsNoTracking().Where(a => a.StudentId == studentId);

        if (subjectId.HasValue)
        {
            var sid = subjectId.Value;
            query = query.Where(a => a.SubjectId == sid);
        }

        if (startDate.HasValue)
        {
            var start = startDate.Value;
            query = query.Where(a => a.AttendanceDate >= start);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value;
            query = query.Where(a => a.AttendanceDate <= end);
        }

        return await query
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync();
    }

    public Task<List<Attendance>> GetBySubjectIdAndDateAsync(Guid subjectId, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return _set.AsNoTracking()
            .Where(a => a.SubjectId == subjectId && a.AttendanceDate >= dayStart && a.AttendanceDate < dayEnd)
            .OrderBy(a => a.AttendanceDate)
            .ToListAsync();
    }
}
