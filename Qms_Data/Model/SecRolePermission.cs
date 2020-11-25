using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecRolePermission
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SecPermission Permission { get; set; }
        public SecRole Role { get; set; }
    }
}
