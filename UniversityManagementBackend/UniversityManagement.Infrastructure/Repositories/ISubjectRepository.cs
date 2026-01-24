using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Infrastructure.Repositories
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(Guid id);
        Task AddAsync(Subject subject);
        Task SaveChangesAsync();
    }
}
