namespace UniversityManagement.Application.DTOs.Attendance;

public class CreateAttendanceDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
