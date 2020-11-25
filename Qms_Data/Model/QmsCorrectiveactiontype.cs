using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsCorrectiveactiontype
    {
        public QmsCorrectiveactiontype()
        {
            QmsCorrectiveactionrequest = new HashSet<QmsCorrectiveactionrequest>();
        }

        public int Id { get; set; }
        public string Label { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<QmsCorrectiveactionrequest> QmsCorrectiveactionrequest { get; set; }
    }
}
