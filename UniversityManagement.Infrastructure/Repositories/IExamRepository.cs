using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.

namespace UniversityManagement.Infrastructure.Repositories;

public interface IExamRepository : IRepository<Exam>
{
    Task<List<Exam>> GetBySubjectIdAsync(Guid subjectId);
}
