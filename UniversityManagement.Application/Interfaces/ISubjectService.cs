using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Subjects;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<List<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(Guid id);
        Task<List<Subject>> GetByProgramIdAsync(Guid programId);

        Task<Subject> CreateAsync(CreateSubjectDto dto);

        Task<Subject?> UpdateAsync(Guid id, UpdateSubjectDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
