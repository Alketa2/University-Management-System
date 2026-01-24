using UniversityManagement.Application.DTOs.Attendance;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Subject> _subjectRepository;

    public AttendanceService(
        IAttendanceRepository attendanceRepository,
        IRepository<Student> studentRepository,
        IRepository<Subject> subjectRepository)
    {
        _attendanceRepository = attendanceRepository;
        _studentRepository = studentRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<AttendanceResponseDto> CreateAttendanceAsync(CreateAttendanceDto createAttendanceDto)
    {
        var attendance = new Attendance
        {
            StudentId = createAttendanceDto.StudentId,
            SubjectId = createAttendanceDto.SubjectId,
            AttendanceDate = createAttendanceDto.AttendanceDate,
            Status = ParseAttendanceStatus(createAttendanceDto.Status),
            Notes = createAttendanceDto.Notes
        };

        var createdAttendance = await _attendanceRepository.AddAsync(attendance);
        return await MapToResponseDto(createdAttendance);
    }

    public async Task<List<AttendanceResponseDto>> CreateBulkAttendanceAsync(BulkAttendanceDto bulkAttendanceDto)
    {
        var result = new List<AttendanceResponseDto>();
        foreach (var studentAttendance in bulkAttendanceDto.StudentAttendances)
        {
            var attendance = new Attendance
            {
                StudentId = studentAttendance.StudentId,
                SubjectId = bulkAttendanceDto.SubjectId,
                AttendanceDate = bulkAttendanceDto.AttendanceDate,
                Status = ParseAttendanceStatus(studentAttendance.Status),
                Notes = studentAttendance.Notes
            };
            var createdAttendance = await _attendanceRepository.AddAsync(attendance);
            result.Add(await MapToResponseDto(createdAttendance));
        }
        return result;
    }

    public async Task<AttendanceResponseDto> UpdateAttendanceAsync(UpdateAttendanceDto updateAttendanceDto)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(updateAttendanceDto.Id);
        if (attendance == null)
            throw new KeyNotFoundException($"Attendance with ID {updateAttendanceDto.Id} not found.");

        attendance.StudentId = updateAttendanceDto.StudentId;
        attendance.SubjectId = updateAttendanceDto.SubjectId;
        attendance.AttendanceDate = updateAttendanceDto.AttendanceDate;
        attendance.Status = ParseAttendanceStatus(updateAttendanceDto.Status);
        attendance.Notes = updateAttendanceDto.Notes;

        var updatedAttendance = await _attendanceRepository.UpdateAsync(attendance);
        return await MapToResponseDto(updatedAttendance);
    }

    public async Task<AttendanceResponseDto?> GetAttendanceByIdAsync(Guid id)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(id);
        return attendance == null ? null : await MapToResponseDto(attendance);
    }

    public async Task<List<AttendanceResponseDto>> GetAttendanceByStudentAsync(Guid studentId, Guid? subjectId, DateTime? startDate, DateTime? endDate)
    {
        var attendances = await _attendanceRepository.GetByStudentIdAsync(studentId, subjectId, startDate, endDate);
        var result = new List<AttendanceResponseDto>();
        foreach (var attendance in attendances)
        {
            result.Add(await MapToResponseDto(attendance));
        }
        return result;
    }

    public async Task<List<AttendanceResponseDto>> GetAttendanceBySubjectAsync(Guid subjectId, DateTime date)
    {
        var attendances = await _attendanceRepository.GetBySubjectIdAndDateAsync(subjectId, date);
        var result = new List<AttendanceResponseDto>();
        foreach (var attendance in attendances)
        {
            result.Add(await MapToResponseDto(attendance));
        }
        return result;
    }

    private static AttendanceStatus ParseAttendanceStatus(string status)
    {
        return Enum.TryParse<AttendanceStatus>(status, true, out var result) ? result : AttendanceStatus.Absent;
    }

    private async Task<AttendanceResponseDto> MapToResponseDto(Attendance attendance)
    {
        var student = await _studentRepository.GetByIdAsync(attendance.StudentId);
        var subject = await _subjectRepository.GetByIdAsync(attendance.SubjectId);

        return new AttendanceResponseDto
        {
            Id = attendance.Id,
            StudentId = attendance.StudentId,
            StudentName = student != null ? $"{student.FirstName} {student.LastName}" : string.Empty,
            SubjectId = attendance.SubjectId,
            SubjectName = subject?.Name ?? string.Empty,
            AttendanceDate = attendance.AttendanceDate,
            Status = attendance.Status.ToString(),
            Notes = attendance.Notes,
            CreatedAt = attendance.CreatedAt,
            UpdatedAt = attendance.UpdatedAt
        };
    }
}
