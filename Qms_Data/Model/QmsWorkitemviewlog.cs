using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsWorkitemviewlog
    {
        public int Id { get; set; }
        public DateTime Createdat { get; set; }
        public int? Workitemid { get; set; }
        public string WorkItemTypeCode { get; set; }
        public int Userid { get; set; }

        public QmsWorkitemtype WorkItemTypeCodeNavigation { get; set; }
    }
}
