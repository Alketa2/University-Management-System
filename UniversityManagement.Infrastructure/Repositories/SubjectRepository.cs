using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly UniversityDbContext _db;

        public SubjectRepository(UniversityDbContext db)
        {
            _db = db;
        }

        public Task<List<Subject>> GetAllAsync()
            => _db.Subjects.AsNoTracking().ToListAsync();

        public Task<Subject?> GetByIdAsync(Guid id)
            => _db.Subjects.FirstOrDefaultAsync(s => s.Id == id);

        public Task<List<Subject>> GetByProgramIdAsync(Guid programId)
            => _db.Subjects.AsNoTracking()
                .Where(s => s.ProgramId == programId)
                .ToListAsync();

        public async Task AddAsync(Subject subject)
        {
            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subject subject)
        {
            _db.Subjects.Update(subject);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Subject subject)
        {
            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
        }
    }
}
