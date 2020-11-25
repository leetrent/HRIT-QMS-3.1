using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsErrorroutingtype
    {
        public QmsErrorroutingtype()
        {
            QmsMasterErrorList = new HashSet<QmsMasterErrorList>();
        }

        public int ErrorRoutingTypeId { get; set; }
        public string ErrorRoutingTypeCode { get; set; }
        public string ErrorRoutingTypeLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<QmsMasterErrorList> QmsMasterErrorList { get; set; }
    }
}
