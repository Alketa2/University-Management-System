namespace UniversityManagement.Application.DTOs.Program;

public class CreateProgramDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public int CreditsRequired { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
