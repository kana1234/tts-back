using System;
using System.Threading.Tasks;
using Charts.Shared.Logic.Models.Notifications;

namespace Charts.Shared.Logic.Notifications
{
    public interface INotificationLogic
    {
        Task<object> AddClientNotification(NotificationInDto model);
        Task<object> AddNotificationEvent(NotificationEventInDto model);
        Task<object> GetClientNotifications(Guid ClientId);
    }
}