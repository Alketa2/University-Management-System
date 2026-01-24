using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly UniversityDbContext _context;

        public SubjectRepository(UniversityDbContext context)
        {
            _context = context;
        }

        public Task<List<Subject>> GetAllAsync()
            => _context.Subjects.AsNoTracking().ToListAsync();

        public Task<Subject?> GetByIdAsync(Guid id)
            => _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
        }

        public Task SaveChangesAsync()
            => _context.SaveChangesAsync();
    }
}
