using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecPermission
    {
        public SecPermission()
        {
            SecRolePermission = new HashSet<SecRolePermission>();
        }

        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SysMenuitem SysMenuitem { get; set; }
        public ICollection<SecRolePermission> SecRolePermission { get; set; }
    }
}
