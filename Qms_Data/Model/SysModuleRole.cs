using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SysModuleRole
    {
        public int ModuleRoleId { get; set; }
        public int ModuleId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SysModule Module { get; set; }
        public SecRole Role { get; set; }
    }
}
