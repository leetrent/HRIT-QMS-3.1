using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecSecuritylogtype
    {
        public SecSecuritylogtype()
        {
            SecSecuritylog = new HashSet<SecSecuritylog>();
        }

        public int SecurityLogTypeId { get; set; }
        public string SecurityLogTypeCode { get; set; }
        public string SecurityLogTypeLabel { get; set; }
        public string SecurityLogTemplate { get; set; }
        public int? SecurityItemTypeId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SecSecurityitemtype SecurityItemType { get; set; }
        public ICollection<SecSecuritylog> SecSecuritylog { get; set; }
    }
}
