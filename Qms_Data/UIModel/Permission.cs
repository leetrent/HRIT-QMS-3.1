using System;
using System.Text;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Permission : ILoggable, IEquatable<Permission>
    {

        public int ID { get {return PermissionId;}}
        public string Label {get {return PermissionLabel;}}
         
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionLabel { get; set; }
        public bool IsActive { get; } = false;

        public string CodeAndLabel
        {
            get { return $"{this.PermissionCode} ({this.PermissionLabel})"; }
        }
        public bool Selected { get; set; } = false;

        public Permission(SecPermission permission)
        {
            PermissionId = permission.PermissionId;
            PermissionCode = permission.PermissionCode;
            PermissionLabel = permission.PermissionLabel;
            IsActive = (permission.DeletedAt == null);
        }

        public Permission(int id, string code, string label)
        {
            this.PermissionId = id;
            this.PermissionCode = code;
            this.PermissionLabel = label;
        }

        public Permission() {}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("permission = { ");
            sb.Append("PermissionId: ");
            sb.Append(this.PermissionId);
            sb.Append(", PermissionCode: ");
            sb.Append(this.PermissionCode);
            sb.Append(", PermissionLabel: ");
            sb.Append(this.PermissionLabel);
            sb.Append(", IsActive: ");
            sb.Append(this.IsActive);
            sb.Append("}");

            return sb.ToString();
        }

        public Permission DeepCopy()
        {
            return new Permission(this.PermissionId, this.PermissionCode, this.PermissionLabel);

        }

        public bool Equals(Permission other)
        {
            return this.ID == other.ID;
        }
    }//end class
}//end permission

