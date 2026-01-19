using UniversityManagement.Domain.Common;    
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<List<Subject>> GetByProgramIdAsync(Guid programId);
}
