using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Subjects;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly UniversityDbContext _db;

        public SubjectService(ISubjectRepository subjectRepository, UniversityDbContext db)
        {
            _subjectRepository = subjectRepository;
            _db = db;
        }

        public Task<List<Subject>> GetAllAsync()
            => _subjectRepository.GetAllAsync();

        public Task<Subject?> GetByIdAsync(Guid id)
            => _subjectRepository.GetByIdAsync(id);

        public Task<List<Subject>> GetByProgramIdAsync(Guid programId)
            => _subjectRepository.GetByProgramIdAsync(programId);

        public async Task<Subject> CreateAsync(CreateSubjectDto dto)
        {
            // FK checks (clean error instead of crash)
            var programExists = await _db.Programs.AnyAsync(p => p.Id == dto.ProgramId);
            if (!programExists) throw new ArgumentException("Program not found");

            var teacherExists = await _db.Teachers.AnyAsync(t => t.Id == dto.TeacherId);
            if (!teacherExists) throw new ArgumentException("Teacher not found");

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Code = dto.Code.Trim(),
                Description = dto.Description,
                Credits = dto.Credits,
                ProgramId = dto.ProgramId,
                TeacherId = dto.TeacherId,
                Semester = dto.Semester,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _subjectRepository.AddAsync(subject);
            return subject;
        }

        public async Task<Subject?> UpdateAsync(Guid id, UpdateSubjectDto dto)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return null;

            subject.Name = dto.Name.Trim();
            subject.Code = dto.Code.Trim();
            subject.Description = dto.Description;
            subject.Credits = dto.Credits;
            subject.Semester = dto.Semester;
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
            return subject;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return false;

            await _subjectRepository.DeleteAsync(subject);
            return true;
        }
    }
}
