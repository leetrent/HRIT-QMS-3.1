using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsMasterErrorList
    {
        public QmsMasterErrorList()
        {
            QmsDataerror = new HashSet<QmsDataerror>();
        }

        public int ErrorListId { get; set; }
        public int DataItemId { get; set; }
        public string QmsErrorCode { get; set; }
        public string HrdwDataLoadEnabled { get; set; }
        public string QmsDataLoadEnabled { get; set; }
        public int ErrorRoutingTypeId { get; set; }
        public int DataRoutingTypeId { get; set; }
        public string ErrorMessageText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public QmsDataItem DataItem { get; set; }
        public QmsDataerrortype DataRoutingType { get; set; }
        public QmsErrorroutingtype ErrorRoutingType { get; set; }
        public ICollection<QmsDataerror> QmsDataerror { get; set; }
    }
}
