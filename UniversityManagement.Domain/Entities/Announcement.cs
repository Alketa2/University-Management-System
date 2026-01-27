using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;
using UniversityManagement.Domain.Entities;

public class Announcement : BaseEntity
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string TargetAudience { get; set; } = "All";


    //  Nullable FK
    public Guid? TeacherId { get; set; }

    //  Navigation property
    public Teacher? Teacher { get; set; }

    public Guid? ProgramId { get; set; }
    public Program? Program { get; set; }

    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}
