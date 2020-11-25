
using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class UserRole : IComparable<UserRole>, IEquatable<UserRole>
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Role Role { get; set; }
        public User User { get; set; }


        public UserRole()
        {
            this.UserRoleId = 0;
        }


        public UserRole(SecUserRole userRole)
        {
            this.UserRoleId = userRole.UserOrgRoleId;
            this.UserId = userRole.UserId;
            this.RoleId = userRole.RoleId;
            this.CreatedAt = userRole.CreatedAt;
            this.DeletedAt = userRole.DeletedAt;
            this.UpdatedAt = userRole.UpdatedAt;

            this.Role = new Role(userRole.Role);
        }

        public SecUserRole SecUserRole()
        {
            SecUserRole secUserRole = new SecUserRole();
            secUserRole.UserOrgRoleId = this.UserRoleId;
            secUserRole.CreatedAt = this.CreatedAt;
            secUserRole.DeletedAt = this.DeletedAt;
            secUserRole.UpdatedAt = this.UpdatedAt;
            secUserRole.RoleId = this.RoleId;
            secUserRole.UserId = this.UserId;
            return secUserRole;
        }

        public int CompareTo(UserRole other)
        {
           return this.UserRoleId.CompareTo(other.UserRoleId);
        }

        public bool Equals(UserRole other)
        {
            return this.UserRoleId == other.UserRoleId;
        }
    }//end class
}//end namespace