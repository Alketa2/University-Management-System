using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Attendance;

public class BulkAttendanceDto
{
    [Required]
    public Guid SubjectId { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one student attendance record is required.")]
    public List<StudentAttendanceDto> StudentAttendances { get; set; } = new();
}

public class StudentAttendanceDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}
