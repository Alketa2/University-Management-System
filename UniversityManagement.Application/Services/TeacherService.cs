using UniversityManagement.Application.DTOs.Teacher;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly IRepository<Teacher> _teacherRepository;

    public TeacherService(IRepository<Teacher> teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<TeacherResponseDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
    {
        var teacher = new Teacher
        {
            FirstName = createTeacherDto.FirstName,
            LastName = createTeacherDto.LastName,
            Email = createTeacherDto.Email,
            Phone = createTeacherDto.Phone,
            Department = createTeacherDto.Department,
            HireDate = createTeacherDto.HireDate,
            Status = TeacherStatus.Active
        };

        var createdTeacher = await _teacherRepository.AddAsync(teacher);
        return MapToResponseDto(createdTeacher);
    }

    public async Task<TeacherResponseDto> UpdateTeacherAsync(UpdateTeacherDto updateTeacherDto)
    {
        var teacher = await _teacherRepository.GetByIdAsync(updateTeacherDto.Id);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {updateTeacherDto.Id} not found.");

        teacher.FirstName = updateTeacherDto.FirstName;
        teacher.LastName = updateTeacherDto.LastName;
        teacher.Email = updateTeacherDto.Email;
        teacher.Phone = updateTeacherDto.Phone;
        teacher.Department = updateTeacherDto.Department;

        if (Enum.TryParse<TeacherStatus>(updateTeacherDto.Status, true, out var status))
        {
            teacher.Status = status;
        }

        var updatedTeacher = await _teacherRepository.UpdateAsync(teacher);
        return MapToResponseDto(updatedTeacher);
    }

    public async Task<TeacherResponseDto?> GetTeacherByIdAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        return teacher == null ? null : MapToResponseDto(teacher);
    }

    public async Task<List<TeacherResponseDto>> GetAllTeachersAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        return teachers.Select(MapToResponseDto).ToList();
    }

    public async Task<bool> DeleteTeacherAsync(Guid id)
    {
        return await _teacherRepository.DeleteAsync(id);
    }

    private static TeacherResponseDto MapToResponseDto(Teacher teacher)
    {
        return new TeacherResponseDto
        {
            Id = teacher.Id,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email,
            Phone = teacher.Phone,
            Department = teacher.Department,
            HireDate = teacher.HireDate,
            Status = teacher.Status.ToString(),
            CreatedAt = teacher.CreatedAt,
            UpdatedAt = teacher.UpdatedAt
        };
    }
}
