using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class ExamRepository : EfRepository<Exam>, IExamRepository
{
    public ExamRepository(UniversityDbContext db) : base(db) { }

    public Task<List<Exam>> GetBySubjectIdAsync(Guid subjectId)
        => _set.AsNoTracking()
            .Where(e => e.SubjectId == subjectId)
            .OrderBy(e => e.ExamDate)
            .ThenBy(e => e.StartTime)
            .ToListAsync();
}
