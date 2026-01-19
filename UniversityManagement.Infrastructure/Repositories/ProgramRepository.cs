using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.


namespace UniversityManagement.Infrastructure.Repositories;

public class ProgramRepository : Repository<Program>
{
}
