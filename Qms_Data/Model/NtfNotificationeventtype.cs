using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class NtfNotificationeventtype
    {
        public NtfNotificationeventtype()
        {
            NtfNotificationevent = new HashSet<NtfNotificationevent>();
        }

        public int NotificationEventTypeId { get; set; }
        public string NotificationEventTypeCode { get; set; }
        public string NotificationEventTypeLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<NtfNotificationevent> NtfNotificationevent { get; set; }
    }
}
