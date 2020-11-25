using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecSecurityitemtype
    {
        public SecSecurityitemtype()
        {
            SecSecuritylogtype = new HashSet<SecSecuritylogtype>();
        }

        public int SecurityItemTypeId { get; set; }
        public string SecurityItemTypeCode { get; set; }
        public string SecurityItemTypeLabel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<SecSecuritylogtype> SecSecuritylogtype { get; set; }
    }
}
