using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class StudentRepository : EfRepository<Student>, IStudentRepository
{
    public StudentRepository(UniversityDbContext db) : base(db) { }

    public Task<List<Student>> GetStudentsByProgramIdAsync(Guid programId)
        => _set.AsNoTracking()
            .Where(s => s.StudentPrograms.Any(sp => sp.ProgramId == programId))
            .ToListAsync();
}
