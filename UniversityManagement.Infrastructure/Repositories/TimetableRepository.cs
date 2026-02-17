using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class TimetableRepository : EfRepository<Timetable>, ITimetableRepository
{
    public TimetableRepository(UniversityDbContext db) : base(db) { }

    public async Task<List<Timetable>> GetByProgramIdAsync(Guid programId, int? semester = null)
    {
        var query = _set.AsNoTracking().Where(t => t.ProgramId == programId);

        if (semester.HasValue)
        {
            var sem = semester.Value;
            query = query.Where(t => t.Semester == sem);
        }

        return await query
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync();
    }
    public async Task<List<Timetable>> GetOverlappingTimetablesAsync(DayOfWeek day, TimeSpan start, TimeSpan end, string? room, Guid? programId, Guid? subjectId, Guid? excludeId = null)
    {
        var query = _set.Include(t => t.Subject).AsNoTracking()
            .Where(t => t.DayOfWeek == day);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        // Time overlap condition: (StartA < EndB) AND (EndA > StartB)
        query = query.Where(t => t.StartTime < end && t.EndTime > start);

        // We want to return anything that matches ANY of the three collision types
        var potentialConflicts = await query.ToListAsync();

        var teacherId = subjectId.HasValue 
            ? (await _db.Subjects.FindAsync(subjectId.Value))?.TeacherId 
            : null;

        return potentialConflicts.Where(t => 
            // 1. Room Collision
            (!string.IsNullOrEmpty(room) && t.Room == room) ||
            // 2. Program Collision
            (programId.HasValue && t.ProgramId == programId.Value) ||
            // 3. Teacher Collision
            (teacherId.HasValue && t.Subject.TeacherId == teacherId.Value)
        ).ToList();
    }
}
