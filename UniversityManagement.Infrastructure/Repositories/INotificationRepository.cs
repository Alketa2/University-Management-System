using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Infrastructure.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
    Task CreateAsync(Notification notification);
    Task UpdateAsync(string id, Notification notification);
    Task DeleteAsync(string id);
    Task<Notification?> GetByIdAsync(string id);
}
