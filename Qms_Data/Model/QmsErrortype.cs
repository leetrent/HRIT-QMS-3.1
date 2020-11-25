using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsErrortype
    {
        public QmsErrortype()
        {
            QmsCorrectiveactionErrortype = new HashSet<QmsCorrectiveactionErrortype>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string RoutesToBr { get; set; }
        public byte DisplayOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<QmsCorrectiveactionErrortype> QmsCorrectiveactionErrortype { get; set; }
    }
}
