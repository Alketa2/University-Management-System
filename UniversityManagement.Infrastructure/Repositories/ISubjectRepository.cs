using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<List<Subject>> GetByProgramIdAsync(Guid programId);
}
