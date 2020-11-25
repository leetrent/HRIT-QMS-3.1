using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsNatureofaction
    {
        public QmsNatureofaction()
        {
            QmsCorrectiveactionrequest = new HashSet<QmsCorrectiveactionrequest>();
        }

        public string Noacode { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string ShortDescription { get; set; }
        public string RoutesToBr { get; set; }

        public ICollection<QmsCorrectiveactionrequest> QmsCorrectiveactionrequest { get; set; }
    }
}
