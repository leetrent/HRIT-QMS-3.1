using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsOrgStatusTrans
    {
        public int OrgStatusTransId { get; set; }
        public int StatusTransId { get; set; }
        public int FromOrgId { get; set; }
        public int ToOrgtypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string WorkItemTypeCode { get; set; }

        public QmsStatusTrans StatusTrans { get; set; }
        public QmsOrgtype ToOrgtype { get; set; }
        public QmsWorkitemtype WorkItemTypeCodeNavigation { get; set; }
    }
}
