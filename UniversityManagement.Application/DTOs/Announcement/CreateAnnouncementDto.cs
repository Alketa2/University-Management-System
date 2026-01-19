namespace UniversityManagement.Application.DTOs.Announcement;

public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public string TargetAudience { get; set; } = string.Empty;
    public Guid? ProgramId { get; set; }
    public Guid? SubjectId { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
