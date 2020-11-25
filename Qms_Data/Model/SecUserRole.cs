using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecUserRole
    {
        public int UserOrgRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public SecRole Role { get; set; }
        public SecUser User { get; set; }
    }
}
