using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class NtfNotification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int NotificationEventId { get; set; }
        public int WorkitemId { get; set; }
        public string WorkItemTypeCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public byte HasBeenRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public byte SendAsEmail { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public NtfNotificationevent NotificationEvent { get; set; }
        public SecUser User { get; set; }
        public QmsWorkitemtype WorkItemTypeCodeNavigation { get; set; }
    }
}
