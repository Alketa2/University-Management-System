namespace UniversityManagement.Application.DTOs.Timetable;

public class UpdateTimetableDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public Guid SubjectId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
}
