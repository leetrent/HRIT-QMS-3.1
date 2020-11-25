using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecRole
    {
        public SecRole()
        {
            SecRolePermission = new HashSet<SecRolePermission>();
            SecUserRole = new HashSet<SecUserRole>();
            SysModuleRole = new HashSet<SysModuleRole>();
        }

        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<SecRolePermission> SecRolePermission { get; set; }
        public ICollection<SecUserRole> SecUserRole { get; set; }
        public ICollection<SysModuleRole> SysModuleRole { get; set; }
    }
}
