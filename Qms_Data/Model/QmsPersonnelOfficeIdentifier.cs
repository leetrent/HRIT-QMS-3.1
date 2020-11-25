using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsPersonnelOfficeIdentifier
    {
        public int PoiId { get; set; }
        public string PoiCode { get; set; }
        public string PoiLabel { get; set; }
        public int OrgId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SecOrg Org { get; set; }
    }
}
