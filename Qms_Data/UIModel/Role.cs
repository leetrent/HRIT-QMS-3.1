using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Role : ILoggable, IEquatable<Role>
    {

        public int ID { get {return RoleId;}}
        public string Label {get {return RoleLabel;}}        
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleLabel { get; set; }   
        public bool IsActive { get; } = false;
         
        public List<Permission> Permissions{get;}
        public List<Permission> CheckboxPermissions{get; set;}


        public Role(SecRole role)
        {
            this.Permissions            = new List<Permission>();
            this.CheckboxPermissions    = new List<Permission>();
            this.RoleCode = role.RoleCode;
            this.RoleId = role.RoleId;
            this.RoleLabel = role.RoleLabel;
            this.IsActive = (role.DeletedAt == null);

            foreach(var rolePermission in role.SecRolePermission)
            {
                if ( (rolePermission.DeletedAt == null) && (rolePermission.Permission.DeletedAt == null) )
                {
                    Permissions.Add(new Permission(rolePermission.Permission));
                }
            }
        } 

        public Role()
        {
            this.Permissions = new List<Permission>();
            this.CheckboxPermissions = new List<Permission>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("role = { ");
            sb.Append("RoleId: ");
            sb.Append(this.RoleId);
            sb.Append(", RoleCode: ");
            sb.Append(this.RoleCode);
            sb.Append(", RoleLabel: ");
            sb.Append(this.RoleLabel);
            sb.Append(", IsActive: ");
            sb.Append(this.IsActive);
            sb.Append(", Permissions: {");
            if (this.Permissions != null)
            {
                foreach (Permission permission in this.Permissions)
                {
                    sb.Append(permission);
                }
            }
            sb.Append("}");
            sb.Append("}");

            return sb.ToString();
        }

        public bool Equals(Role other)
        {
            return this.ID == other.ID;
        }
    }
}