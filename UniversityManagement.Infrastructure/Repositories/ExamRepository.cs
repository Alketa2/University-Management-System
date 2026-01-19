using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Infrastructure.Repositories;

public class ExamRepository
    : Repository<Exam>, IExamRepository
{
    public Task<List<Exam>> GetBySubjectIdAsync(Guid subjectId)
    {
        var exams = _entities.Values
            .Where(e => e.SubjectId == subjectId)
            .ToList();

        return Task.FromResult(exams);
    }
}
