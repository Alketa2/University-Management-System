using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Subject;
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

        public async Task<List<SubjectResponseDto>> GetAllAsync()
        {
            var subjects = await _db.Subjects
                .Include(s => s.Program)
                .Include(s => s.Teacher)
                .AsNoTracking()
                .ToListAsync();

            return subjects.Select(MapToResponseDto).ToList();
        }

        public async Task<SubjectResponseDto?> GetByIdAsync(Guid id)
        {
            var subject = await _db.Subjects
                .Include(s => s.Program)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            return subject == null ? null : MapToResponseDto(subject);
        }

        public async Task<List<SubjectResponseDto>> GetByProgramIdAsync(Guid programId)
        {
            var subjects = await _db.Subjects
                .Include(s => s.Program)
                .Include(s => s.Teacher)
                .Where(s => s.ProgramId == programId)
                .AsNoTracking()
                .ToListAsync();

            return subjects.Select(MapToResponseDto).ToList();
        }

        public async Task<SubjectResponseDto> CreateAsync(CreateSubjectDto dto)
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

            // Reload to get navigation properties for the DTO
            return (await GetByIdAsync(subject.Id))!;
        }

        public async Task<SubjectResponseDto?> UpdateAsync(Guid id, UpdateSubjectDto dto)
        {
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null) return null;

            subject.Name = dto.Name.Trim();
            subject.Code = dto.Code.Trim();
            subject.Description = dto.Description;
            subject.Credits = dto.Credits;
            subject.Semester = dto.Semester;
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
            
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return false;

            await _subjectRepository.DeleteAsync(subject);
            return true;
        }

        private SubjectResponseDto MapToResponseDto(Subject subject)
        {
            return new SubjectResponseDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Code = subject.Code,
                Description = subject.Description,
                Credits = subject.Credits,
                ProgramId = subject.ProgramId,
                ProgramName = subject.Program?.Name ?? "Unknown",
                TeacherId = subject.TeacherId,
                TeacherName = subject.Teacher != null ? $"{subject.Teacher.FirstName} {subject.Teacher.LastName}" : "Unknown",
                Semester = subject.Semester,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt
            };
        }
    }
}
