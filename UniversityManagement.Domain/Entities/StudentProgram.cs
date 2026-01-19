using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities;

public class StudentProgram : BaseEntity
{
    public Guid StudentId { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Program Program { get; set; } = null!;
}
