using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class NtfNotificationuserpreference
    {
        public int NotificationUserPreferenceId { get; set; }
        public int UserId { get; set; }
        public int NotificationEventId { get; set; }
        public byte MessageDeliveryIsOn { get; set; }
        public byte CanBeTurnedOffByUser { get; set; }

        public NtfNotificationevent NotificationEvent { get; set; }
        public SecUser User { get; set; }
    }
}
