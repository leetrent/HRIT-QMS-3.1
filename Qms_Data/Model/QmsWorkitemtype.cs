using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsWorkitemtype
    {
        public QmsWorkitemtype()
        {
            NtfNotification = new HashSet<NtfNotification>();
            QmsOrgStatusTrans = new HashSet<QmsOrgStatusTrans>();
            QmsWorkitemcomment = new HashSet<QmsWorkitemcomment>();
            QmsWorkitemfile = new HashSet<QmsWorkitemfile>();
            QmsWorkitemhistory = new HashSet<QmsWorkitemhistory>();
            QmsWorkitemviewlog = new HashSet<QmsWorkitemviewlog>();
        }

        public int WorkItemTypeId { get; set; }
        public string WorkItemTypeCode { get; set; }
        public string WorkItemTypeLabel { get; set; }
        public string ControllerName { get; set; }
        public string MethodName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<NtfNotification> NtfNotification { get; set; }
        public ICollection<QmsOrgStatusTrans> QmsOrgStatusTrans { get; set; }
        public ICollection<QmsWorkitemcomment> QmsWorkitemcomment { get; set; }
        public ICollection<QmsWorkitemfile> QmsWorkitemfile { get; set; }
        public ICollection<QmsWorkitemhistory> QmsWorkitemhistory { get; set; }
        public ICollection<QmsWorkitemviewlog> QmsWorkitemviewlog { get; set; }
    }
}
