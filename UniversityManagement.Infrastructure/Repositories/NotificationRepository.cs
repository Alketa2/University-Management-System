using MongoDB.Driver;
using UniversityManagement.Domain.MongoEntities;
using UniversityManagement.Infrastructure.Data.Mongo;

namespace UniversityManagement.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly MongoDbContext _context;

    public NotificationRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Find(n => n.RecipientId == userId)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Notification notification)
    {
        await _context.Notifications.InsertOneAsync(notification);
    }

    public async Task UpdateAsync(string id, Notification notification)
    {
        await _context.Notifications.ReplaceOneAsync(n => n.Id == id, notification);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Notifications.DeleteOneAsync(n => n.Id == id);
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _context.Notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
    }
}
