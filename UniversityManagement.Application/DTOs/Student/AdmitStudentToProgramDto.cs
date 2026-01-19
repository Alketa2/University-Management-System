namespace UniversityManagement.Application.DTOs.Student;

public class AdmitStudentToProgramDto
{
    public Guid StudentId { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime? AdmissionDate { get; set; }
}
