using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(string notificationId);
    Task DeleteNotificationAsync(string notificationId);
}
