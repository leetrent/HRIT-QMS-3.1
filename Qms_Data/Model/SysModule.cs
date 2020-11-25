using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SysModule
    {
        public SysModule()
        {
            SysMenuitem = new HashSet<SysMenuitem>();
            SysModuleRole = new HashSet<SysModuleRole>();
        }

        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleLabel { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string QueryString { get; set; }
        public byte DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<SysMenuitem> SysMenuitem { get; set; }
        public ICollection<SysModuleRole> SysModuleRole { get; set; }
    }
}
