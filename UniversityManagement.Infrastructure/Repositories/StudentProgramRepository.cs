using UniversityManagement.Domain.Common;     
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   

namespace UniversityManagement.Infrastructure.Repositories;

public class StudentProgramRepository : Repository<StudentProgram>, IStudentProgramRepository
{
    public Task<List<StudentProgram>> GetByStudentIdAsync(Guid studentId)
    {
        var studentPrograms = _entities.Values
            .Where(sp => sp.StudentId == studentId)
            .ToList();
        return Task.FromResult(studentPrograms);
    }

    public Task<List<StudentProgram>> GetByProgramIdAsync(Guid programId)
    {
        var studentPrograms = _entities.Values
            .Where(sp => sp.ProgramId == programId)
            .ToList();
        return Task.FromResult(studentPrograms);
    }

    public Task<StudentProgram?> GetByStudentAndProgramAsync(Guid studentId, Guid programId)
    {
        var studentProgram = _entities.Values
            .FirstOrDefault(sp => sp.StudentId == studentId && sp.ProgramId == programId);
        return Task.FromResult(studentProgram);
    }
}
