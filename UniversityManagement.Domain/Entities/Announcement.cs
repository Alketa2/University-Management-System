using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class Announcement : BaseEntity
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid TeacherId { get; set; }
    public TargetAudience TargetAudience { get; set; }
    public Guid? ProgramId { get; set; }
    public Guid? SubjectId { get; set; }
    [Required]
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Teacher Teacher { get; set; } = null!;
    public Program? Program { get; set; }
    public Subject? Subject { get; set; }
}

public enum TargetAudience
{
    All = 1,
    Program = 2,
    Subject = 3
}
