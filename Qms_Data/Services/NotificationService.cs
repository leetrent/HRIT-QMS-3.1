using System;
using System.Collections.Generic;
using QmsCore.UIModel;
using System.Net.Mail;
using System.Linq;
using QmsCore.Repository;
using QmsCore.QmsException;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;

namespace QmsCore.Services
{
    public class NotificationService : INotificationService
    {
        NotificationRepository notificationRepository;

        public NotificationService()
        {
            notificationRepository = new NotificationRepository();
        }

        public NotificationService(QMSContext qmsContext)
        {
            notificationRepository = new NotificationRepository(qmsContext);
        }

        public List<Notification> RetrieveUserNotifications(int userId, bool onlyUnread)
        {
            List<Notification> retval = new List<Notification>();
            var results = notificationRepository.RetrieveNotificationByUserId(userId,onlyUnread);
            foreach(var result in results)
            {
                retval.Add(new Notification(result));
            }
            return retval;
        }

        public int RetrieveNotificationCountByUserId(int userId, bool onlyRead)
        {
            return notificationRepository.RetrieveNotificationCountByUserId(userId,onlyRead);
        }


        public List<Notification> RetrieveNotificationForDistribution()
        {
            List<Notification> notifications = new List<Notification>();
            var results = notificationRepository.RetrieveNotificationForDistribution();
            foreach(var result in results)
            {
                notifications.Add(new Notification(result));
            }
            return notifications;

        }

        public WorkItemType MarkAsRead(Notification notification)
        {
            return MarkAsRead(notification.NotificationId);
        }

        public void MarkAsRead(int[] notificationIds)
        {
            foreach(int notificationId in notificationIds)
            {
                NtfNotification n = notificationRepository.RetrieveNotificationById(notificationId);
                n.ReadAt = DateTime.Now;
                n.HasBeenRead = 1;
                notificationRepository.Update(n);                
            }
        }

        public WorkItemType MarkAsRead(int NotificationId)
        {
            NtfNotification n = notificationRepository.RetrieveNotificationById(NotificationId);
            //WorkItemType retval = new WorkItemType(n.WorkItemTypeCodeNavigation,NotificationId);
            WorkItemType retval = new WorkItemType(n.WorkItemTypeCodeNavigation,n.WorkitemId);
            n.ReadAt = DateTime.Now;
            n.HasBeenRead = 1;
            notificationRepository.Update(n);
            return retval;
        }

        public void MarkAsSent(int NotificationId)
        {
            NtfNotification n = notificationRepository.RetrieveNotificationById(NotificationId);
            n.SentAt = DateTime.Now;
            notificationRepository.Update(n);
        }

        public void Insert(Notification notification)
        {
            notificationRepository.Insert(notification.NtfNotification());
        }

        public void Delete(Notification notification)
        {
            NtfNotification ntfNotification = notification.NtfNotification();
            notificationRepository.Delete(ntfNotification);
        }

        public void Delete(List<Notification> notifications)
        {
            foreach(Notification notification in notifications)
            {
                Delete(notification);
            }
        }

        public void Delete(int notificationId)
        {
            NtfNotification ntfNotification = notificationRepository.RetrieveNotificationById(notificationId);
            notificationRepository.Delete(ntfNotification);
        }

        public void Delete(int[] notificationIds)
        {
            foreach(int notificationId in notificationIds)
            {
                Delete(notificationId);
            }
        }



        public void SendEmail(string subjectLine, string messageBody)
        {
            MailMessage message = new MailMessage();
            message.To.Add("lee.trent@gsa.gov");
            message.To.Add("alfred.ortega@gsa.gov");
            message.Subject = subjectLine;
            message.Body = messageBody;
            message.IsBodyHtml = false;
            message.From = new MailAddress("no-reply@gsa.gov");
            SendEmail(message);

        }

        public void SendEmail(MailMessage message)
        {
            SmtpClient client = new SmtpClient("smtp.gsa.gov");
            client.UseDefaultCredentials = true;
            client.Send(message);
        }





    }//end class
}//end namespace