using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<List<Attendance>> GetByStudentIdAsync(Guid studentId, Guid? subjectId, DateTime? startDate, DateTime? endDate);
    Task<List<Attendance>> GetBySubjectIdAndDateAsync(Guid subjectId, DateTime date);
}
