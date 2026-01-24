using UniversityManagement.Domain.Common;    
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<List<Attendance>> GetByStudentIdAsync(Guid studentId, Guid? subjectId, DateTime? startDate, DateTime? endDate);
    Task<List<Attendance>> GetBySubjectIdAndDateAsync(Guid subjectId, DateTime date);
}
