using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<List<Student>> GetStudentsByProgramIdAsync(Guid programId);
}
