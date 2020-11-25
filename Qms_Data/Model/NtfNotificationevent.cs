using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class NtfNotificationevent
    {
        public NtfNotificationevent()
        {
            NtfNotification = new HashSet<NtfNotification>();
            NtfNotificationuserpreference = new HashSet<NtfNotificationuserpreference>();
        }

        public int NotificationEventId { get; set; }
        public string NotificationEventCode { get; set; }
        public string NotificationEventLabel { get; set; }
        public int? NotificationEventTypeId { get; set; }
        public string MessageTemplate { get; set; }
        public string TitleTemplate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public NtfNotificationeventtype NotificationEventType { get; set; }
        public ICollection<NtfNotification> NtfNotification { get; set; }
        public ICollection<NtfNotificationuserpreference> NtfNotificationuserpreference { get; set; }
    }
}
