using UniversityManagement.Domain.Common;     
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   
namespace UniversityManagement.Infrastructure.Repositories;

public interface IExamRepository : IRepository<Exam>
{
    Task<List<Exam>> GetBySubjectIdAsync(Guid subjectId);
}
