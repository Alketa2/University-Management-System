using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories
{
    
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly UniversityDbContext _db;

        public AnnouncementRepository(UniversityDbContext db)
        {
            _db = db;
        }

        public async Task<List<Announcement>> GetAllAsync()
        {
            return await _db.Announcements
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(Guid id)
        {
            return await _db.Announcements
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Announcement> AddAsync(Announcement entity)
        {
            _db.Announcements.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

      
        public async Task<Announcement> UpdateAsync(Announcement entity)
        {
            _db.Announcements.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Announcements.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return false;

            _db.Announcements.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

       
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _db.Announcements.AnyAsync(a => a.Id == id);
        }


        public async Task<List<Announcement>> GetActiveAnnouncementsAsync(Guid? programId, Guid? subjectId)
        {
            var now = DateTime.UtcNow;

            var q = _db.Announcements
                .AsNoTracking()
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > now));

            
            if (programId.HasValue)
            {
                q = q.Where(a =>
                    (a.TargetAudience == null || a.TargetAudience == "" || a.TargetAudience == "All") ||
                    (a.TargetAudience == "Program" && a.ProgramId == programId));
            }

            if (subjectId.HasValue)
            {
                q = q.Where(a =>
                    (a.TargetAudience == null || a.TargetAudience == "" || a.TargetAudience == "All") ||
                    (a.TargetAudience == "Subject" && a.SubjectId == subjectId));
            }

            return await q.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<List<Announcement>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await _db.Announcements
                .AsNoTracking()
                .Where(a => a.TeacherId == teacherId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
