using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Notification
    {
        private byte trueByte = 1;
        private byte falseByte = 0;


        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int WorkitemId { get; set; }
        public string WorkItemType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool HasBeenRead { get; set; }

        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? SentAt {get;set;}

        public DateTime? DeletedAt {get;set;}        

        public int NotificationEventId {get;set;}

        public bool SendAsEmail {get;set;}

        public User NotificationRecipient {get; private set;}

        public Notification(QmsCore.Model.NtfNotification notification)
        {
            this.NotificationId = notification.NotificationId;
            this.UserId = notification.UserId;
            this.WorkitemId = notification.WorkitemId;
            this.WorkItemType = notification.WorkItemTypeCode;
            this.Title = notification.Title;
            this.Message = notification.Message;
            this.HasBeenRead = notification.HasBeenRead == 1;
            this.SendAsEmail = notification.SendAsEmail == 1;
            this.ReadAt = notification.ReadAt;
            this.CreatedAt = notification.CreatedAt;
            this.NotificationRecipient = new User(notification.User,false,false);
            this.SentAt = notification.SentAt;
            this.DeletedAt = notification.DeletedAt;

        }

        public Notification()
        {
            CreatedAt = DateTime.Now;
            SentAt = DateTime.Now;

        }

        public NtfNotification NtfNotification()
        {
            NtfNotification notification = new NtfNotification();
            notification.CreatedAt = this.CreatedAt;
            notification.DeletedAt = this.DeletedAt;
            notification.HasBeenRead = (this.HasBeenRead) ? trueByte : falseByte;
            notification.Message = this.Message;
            notification.NotificationEventId = this.NotificationEventId;
            notification.NotificationId = this.NotificationId;
            notification.ReadAt = this.ReadAt;
            notification.SendAsEmail  = (this.SendAsEmail) ? trueByte : falseByte;
            notification.SentAt = this.SentAt;
            notification.Title = this.Title;
            notification.UserId = this.UserId;
            notification.WorkitemId = this.WorkitemId;
            notification.WorkItemTypeCode = this.WorkItemType;
            return notification;

        }

    }//end class
}//end namespace