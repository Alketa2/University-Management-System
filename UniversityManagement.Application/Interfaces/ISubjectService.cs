using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Subject;
using UniversityManagement.Application.DTOs.Subjects;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectResponseDto>> GetAllAsync();
        Task<SubjectResponseDto?> GetByIdAsync(Guid id);
        Task<List<SubjectResponseDto>> GetByProgramIdAsync(Guid programId);

        Task<SubjectResponseDto> CreateAsync(CreateSubjectDto dto);

        Task<SubjectResponseDto?> UpdateAsync(Guid id, UpdateSubjectDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
