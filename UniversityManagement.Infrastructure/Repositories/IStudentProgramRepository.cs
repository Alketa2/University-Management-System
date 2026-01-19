using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public interface IStudentProgramRepository : IRepository<StudentProgram>
{
    Task<List<StudentProgram>> GetByStudentIdAsync(Guid studentId);
    Task<List<StudentProgram>> GetByProgramIdAsync(Guid programId);
    Task<StudentProgram?> GetByStudentAndProgramAsync(Guid studentId, Guid programId);
}
