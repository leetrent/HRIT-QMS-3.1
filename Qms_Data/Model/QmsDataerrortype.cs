using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsDataerrortype
    {
        public QmsDataerrortype()
        {
            QmsMasterErrorList = new HashSet<QmsMasterErrorList>();
        }

        public int DataRoutingTypeId { get; set; }
        public string DataRoutingTypeCode { get; set; }
        public string DataRoutingTypeLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<QmsMasterErrorList> QmsMasterErrorList { get; set; }
    }
}
