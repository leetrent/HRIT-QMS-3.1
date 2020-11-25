using System.Net.Mail;
using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface INotificationService
    {
        List<Notification> RetrieveUserNotifications(int userId, bool onlyUnread);
        List<Notification> RetrieveNotificationForDistribution();
        void MarkAsRead(int[] notificationIds);
        WorkItemType MarkAsRead(int NotificationId);
        void MarkAsSent(int NotificationId);
        void Insert(Notification notification);
        void Delete(Notification notification);
        void Delete(List<Notification> notifications);
        void Delete(int notificationId);
        void Delete(int[] notificationIds);
        void SendEmail(string subjectLine, string messageBody);
        void SendEmail(MailMessage message);
    }
}