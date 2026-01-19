using UniversityManagement.Application.DTOs.Attendance;

namespace UniversityManagement.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceResponseDto> CreateAttendanceAsync(CreateAttendanceDto createAttendanceDto);
    Task<List<AttendanceResponseDto>> CreateBulkAttendanceAsync(BulkAttendanceDto bulkAttendanceDto);
    Task<AttendanceResponseDto> UpdateAttendanceAsync(UpdateAttendanceDto updateAttendanceDto);
    Task<AttendanceResponseDto?> GetAttendanceByIdAsync(Guid id);
    Task<List<AttendanceResponseDto>> GetAttendanceByStudentAsync(Guid studentId, Guid? subjectId, DateTime? startDate, DateTime? endDate);
    Task<List<AttendanceResponseDto>> GetAttendanceBySubjectAsync(Guid subjectId, DateTime date);
}
