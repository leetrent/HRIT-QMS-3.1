using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsOrgtype
    {
        public QmsOrgtype()
        {
            QmsOrgStatusTrans = new HashSet<QmsOrgStatusTrans>();
        }

        public int OrgtypeId { get; set; }
        public string OrgtypeLabel { get; set; }
        public string OrgtypeCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<QmsOrgStatusTrans> QmsOrgStatusTrans { get; set; }
    }
}
