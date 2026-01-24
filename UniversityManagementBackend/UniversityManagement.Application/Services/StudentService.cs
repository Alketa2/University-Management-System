using UniversityManagement.Application.DTOs.Program;
using UniversityManagement.Application.DTOs.Student;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IStudentProgramRepository _studentProgramRepository;
    private readonly IRepository<Program> _programRepository;

    public StudentService(
        IRepository<Student> studentRepository,
        IStudentProgramRepository studentProgramRepository,
        IRepository<Program> programRepository)
    {
        _studentRepository = studentRepository;
        _studentProgramRepository = studentProgramRepository;
        _programRepository = programRepository;
    }

    public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        var student = new Student
        {
            FirstName = createStudentDto.FirstName,
            LastName = createStudentDto.LastName,
            Email = createStudentDto.Email,
            Phone = createStudentDto.Phone,
            DateOfBirth = createStudentDto.DateOfBirth,
            Address = createStudentDto.Address,
            EnrollmentDate = DateTime.UtcNow,
            Status = StudentStatus.Enrolled
        };

        var createdStudent = await _studentRepository.AddAsync(student);
        return MapToResponseDto(createdStudent);
    }

    public async Task<StudentResponseDto> UpdateStudentAsync(UpdateStudentDto updateStudentDto)
    {
        var student = await _studentRepository.GetByIdAsync(updateStudentDto.Id);
        if (student == null)
            throw new KeyNotFoundException($"Student with ID {updateStudentDto.Id} not found.");

        student.FirstName = updateStudentDto.FirstName;
        student.LastName = updateStudentDto.LastName;
        student.Email = updateStudentDto.Email;
        student.Phone = updateStudentDto.Phone;
        student.DateOfBirth = updateStudentDto.DateOfBirth;
        student.Address = updateStudentDto.Address;
        
        if (Enum.TryParse<StudentStatus>(updateStudentDto.Status, true, out var status))
        {
            student.Status = status;
        }

        var updatedStudent = await _studentRepository.UpdateAsync(student);
        return MapToResponseDto(updatedStudent);
    }

    public async Task<StudentResponseDto?> GetStudentByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        return student == null ? null : MapToResponseDto(student);
    }

    public async Task<List<StudentResponseDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(MapToResponseDto).ToList();
    }

    public async Task<bool> DeleteStudentAsync(Guid id)
    {
        return await _studentRepository.DeleteAsync(id);
    }

    public async Task<bool> AdmitStudentToProgramAsync(AdmitStudentToProgramDto admitDto)
    {
        var student = await _studentRepository.GetByIdAsync(admitDto.StudentId);
        var program = await _programRepository.GetByIdAsync(admitDto.ProgramId);

        if (student == null || program == null)
            return false;

        var existing = await _studentProgramRepository.GetByStudentAndProgramAsync(admitDto.StudentId, admitDto.ProgramId);
        if (existing != null)
            return false; // Already admitted

        var studentProgram = new StudentProgram
        {
            StudentId = admitDto.StudentId,
            ProgramId = admitDto.ProgramId,
            AdmissionDate = admitDto.AdmissionDate ?? DateTime.UtcNow
        };

        await _studentProgramRepository.AddAsync(studentProgram);
        return true;
    }

    public async Task<List<ProgramResponseDto>> GetStudentProgramsAsync(Guid studentId)
    {
        var studentPrograms = await _studentProgramRepository.GetByStudentIdAsync(studentId);
        var programIds = studentPrograms.Select(sp => sp.ProgramId).ToList();
        
        var programs = await _programRepository.GetAllAsync();
        var studentProgramsList = programs.Where(p => programIds.Contains(p.Id)).ToList();

        return studentProgramsList.Select(MapToProgramResponseDto).ToList();
    }

    private static StudentResponseDto MapToResponseDto(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            EnrollmentDate = student.EnrollmentDate,
            Status = student.Status.ToString(),
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }

    private static ProgramResponseDto MapToProgramResponseDto(Program program)
    {
        return new ProgramResponseDto
        {
            Id = program.Id,
            Name = program.Name,
            Code = program.Code,
            Description = program.Description,
            Duration = program.Duration,
            CreditsRequired = program.CreditsRequired,
            StartDate = program.StartDate,
            EndDate = program.EndDate,
            IsActive = program.IsActive,
            CreatedAt = program.CreatedAt,
            UpdatedAt = program.UpdatedAt
        };
    }
}
