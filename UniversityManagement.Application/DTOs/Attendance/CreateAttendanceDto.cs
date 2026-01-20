using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Attendance;

public class CreateAttendanceDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}
