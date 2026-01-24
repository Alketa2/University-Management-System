using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class StudentProgramRepository : EfRepository<StudentProgram>, IStudentProgramRepository
{
    public StudentProgramRepository(UniversityDbContext db) : base(db) { }

    public Task<List<StudentProgram>> GetByStudentIdAsync(Guid studentId)
        => _set.AsNoTracking()
            .Where(sp => sp.StudentId == studentId)
            .ToListAsync();

    public Task<List<StudentProgram>> GetByProgramIdAsync(Guid programId)
        => _set.AsNoTracking()
            .Where(sp => sp.ProgramId == programId)
            .ToListAsync();

    public Task<StudentProgram?> GetByStudentAndProgramAsync(Guid studentId, Guid programId)
        => _set.AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ProgramId == programId);
}
