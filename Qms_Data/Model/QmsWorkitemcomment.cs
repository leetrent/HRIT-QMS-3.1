using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsWorkitemcomment
    {
        public int Id { get; set; }
        public int? WorkItemId { get; set; }
        public string WorkItemTypeCode { get; set; }
        public string Message { get; set; }
        public int? AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SecUser Author { get; set; }
        public QmsWorkitemtype WorkItemTypeCodeNavigation { get; set; }
    }
}
