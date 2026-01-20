namespace UniversityManagement.Application.DTOs.Announcement;

public class AnnouncementResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public string TargetAudience { get; set; } = string.Empty;

    public Guid? ProgramId { get; set; }
    public string? ProgramName { get; set; }

    public Guid? SubjectId { get; set; }
    public string? SubjectName { get; set; }

    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
