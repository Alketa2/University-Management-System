using UniversityManagement.Domain.Common;    
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public interface ITimetableRepository : IRepository<Timetable>
{
    Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null);
}
