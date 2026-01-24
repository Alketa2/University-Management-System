using UniversityManagement.Application.DTOs.Program;
using UniversityManagement.Application.DTOs.Student;

namespace UniversityManagement.Application.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createStudentDto);
    Task<StudentResponseDto> UpdateStudentAsync(UpdateStudentDto updateStudentDto);
    Task<StudentResponseDto?> GetStudentByIdAsync(Guid id);
    Task<List<StudentResponseDto>> GetAllStudentsAsync();
    Task<bool> DeleteStudentAsync(Guid id);
    Task<bool> AdmitStudentToProgramAsync(AdmitStudentToProgramDto admitDto);
    Task<List<ProgramResponseDto>> GetStudentProgramsAsync(Guid studentId);
}
