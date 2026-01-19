using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Infrastructure.Repositories;

public class AttendanceRepository
    : Repository<Attendance>, IAttendanceRepository
{
    public Task<List<Attendance>> GetByStudentIdAsync(
        Guid studentId,
        Guid? subjectId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _entities.Values.Where(a => a.StudentId == studentId);

        if (subjectId.HasValue)
            query = query.Where(a => a.SubjectId == subjectId.Value);

        if (startDate.HasValue)
            query = query.Where(a => a.AttendanceDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.AttendanceDate <= endDate.Value);

        return Task.FromResult(query.ToList());
    }

    public Task<List<Attendance>> GetBySubjectIdAndDateAsync(Guid subjectId, DateTime date)
    {
        var attendances = _entities.Values
            .Where(a =>
                a.SubjectId == subjectId &&
                a.AttendanceDate.Date == date.Date)
            .ToList();

        return Task.FromResult(attendances);
    }
}
