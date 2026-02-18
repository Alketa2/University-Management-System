using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.MongoEntities;
using UniversityManagement.Infrastructure.Repositories;

namespace UniversityManagement.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        await _repository.CreateAsync(notification);
        return notification;
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var notification = await _repository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _repository.UpdateAsync(notificationId, notification);
        }
    }

    public async Task DeleteNotificationAsync(string notificationId)
    {
        await _repository.DeleteAsync(notificationId);
    }
}
