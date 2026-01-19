using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public interface ITimetableRepository : IRepository<Timetable>
{
    Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null);
}
