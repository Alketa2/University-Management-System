namespace UniversityManagement.Application.DTOs.Subject;

public class UpdateSubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public Guid ProgramId { get; set; }
    public Guid TeacherId { get; set; }
    public int Semester { get; set; }
    public bool IsActive { get; set; }
}
