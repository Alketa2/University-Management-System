using UniversityManagement.Domain.Common;     // for BaseEntity
using UniversityManagement.Domain.Interfaces; // for IRepository<T>
using UniversityManagement.Domain.Entities;   // for Announcement, StudentProgram, etc.

namespace UniversityManagement.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    private readonly IStudentProgramRepository _studentProgramRepository;

    public StudentRepository(IStudentProgramRepository studentProgramRepository)
    {
        _studentProgramRepository = studentProgramRepository;
    }

    public async Task<List<Student>> GetStudentsByProgramIdAsync(Guid programId)
    {
        // Get all student-program relationships for this program
        var studentPrograms = await _studentProgramRepository.GetByProgramIdAsync(programId);
        var studentIds = studentPrograms.Select(sp => sp.StudentId).ToList();

        // Get all students and filter by the IDs
        var allStudents = await GetAllAsync();
        return allStudents.Where(s => studentIds.Contains(s.Id)).ToList();
    }
}
