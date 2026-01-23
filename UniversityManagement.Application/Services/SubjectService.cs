using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Subjects;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;

        public SubjectService(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Subject>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Subject?> GetByIdAsync(Guid id)
            => _repo.GetByIdAsync(id);

        public async Task<Subject> CreateAsync(CreateSubjectDto dto)
        {
            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Credits = dto.Credits,
                ProgramId = dto.ProgramId,
                TeacherId = dto.TeacherId,
                Semester = dto.Semester,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _repo.AddAsync(subject);
            await _repo.SaveChangesAsync();

            return subject;
        }
    }
}
