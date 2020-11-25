using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsDataItem
    {
        public QmsDataItem()
        {
            QmsMasterErrorList = new HashSet<QmsMasterErrorList>();
        }

        public int DataItemId { get; set; }
        public string SystemName { get; set; }
        public string DataItemName { get; set; }
        public string DataItemCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<QmsMasterErrorList> QmsMasterErrorList { get; set; }
    }
}
