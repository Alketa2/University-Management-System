using UniversityManagement.Domain.Common;     
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<List<Student>> GetStudentsByProgramIdAsync(Guid programId);
}
