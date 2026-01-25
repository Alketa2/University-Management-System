using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Domain.Interfaces
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(Guid id);
        Task<List<Subject>> GetByProgramIdAsync(Guid programId);

        Task AddAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(Subject subject);
    }
}
