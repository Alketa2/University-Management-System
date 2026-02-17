using UniversityManagement.Domain.Common;    
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public interface ITimetableRepository : IRepository<Timetable>
{
    Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null);
    Task<List<Timetable>> GetOverlappingTimetablesAsync(DayOfWeek day, TimeSpan start, TimeSpan end, string? room, Guid? programId, Guid? subjectId, Guid? excludeId = null);
}
