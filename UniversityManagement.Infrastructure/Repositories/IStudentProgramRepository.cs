using UniversityManagement.Domain.Common;     
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;  


namespace UniversityManagement.Infrastructure.Repositories;

public interface IStudentProgramRepository : IRepository<StudentProgram>
{
    Task<List<StudentProgram>> GetByStudentIdAsync(Guid studentId);
    Task<List<StudentProgram>> GetByProgramIdAsync(Guid programId);
    Task<StudentProgram?> GetByStudentAndProgramAsync(Guid studentId, Guid programId);
}
