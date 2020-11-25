using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsWorkitemfile
    {
        public int Id { get; set; }
        public int? UploadedByUserId { get; set; }
        public int WorkItemId { get; set; }
        public string WorkItemTypeCode { get; set; }
        public string Filepath { get; set; }
        public string Filetype { get; set; }
        public DateTime Createdat { get; set; }
        public DateTime Deletedat { get; set; }

        public SecUser UploadedByUser { get; set; }
        public QmsWorkitemtype WorkItemTypeCodeNavigation { get; set; }
    }
}
