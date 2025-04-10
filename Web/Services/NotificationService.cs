using Web.Components.Notification;

namespace Web.Services;

public class NotificationService
{
#pragma warning disable CA1003
    public event Func<string, Notification.NotificationType, Task>? OnShow;
#pragma warning restore CA1003

    public Task Show(string message, Notification.NotificationType type)
    {
        return OnShow?.Invoke(message, type) ?? Task.CompletedTask;
    }
}
