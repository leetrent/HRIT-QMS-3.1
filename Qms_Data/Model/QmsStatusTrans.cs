using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsStatusTrans
    {
        public QmsStatusTrans()
        {
            QmsOrgStatusTrans = new HashSet<QmsOrgStatusTrans>();
        }

        public int StatusTransId { get; set; }
        public int FromStatusId { get; set; }
        public int ToStatusId { get; set; }
        public string StatusTransCode { get; set; }
        public string StatusTransLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public QmsStatus FromStatus { get; set; }
        public QmsStatus ToStatus { get; set; }
        public ICollection<QmsOrgStatusTrans> QmsOrgStatusTrans { get; set; }
    }
}
