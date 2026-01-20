using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Announcement;

public class CreateAnnouncementDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid TeacherId { get; set; }

    [Required]
    [StringLength(50)]
    public string TargetAudience { get; set; } = string.Empty;

    public Guid? ProgramId { get; set; }
    public Guid? SubjectId { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
