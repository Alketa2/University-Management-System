namespace UniversityManagement.Application.DTOs.Attendance;

public class BulkAttendanceDto
{
    public Guid SubjectId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public List<StudentAttendanceDto> StudentAttendances { get; set; } = new();
}

public class StudentAttendanceDto
{
    public Guid StudentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
