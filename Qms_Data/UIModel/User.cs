using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class User : ILoggable
    {
        public int ID { get {return UserId;}}
        public string Label {get {return DisplayName;}}

        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public int? OrgId { get; set; }
        public string OrganizationName {get;set;}
        public int? ManagerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string ManagerName {get;set;}
        public virtual Organization Organization { get; set; }  
        public List<UserRole> UserRoles{get;set;}

        public string DisplayLabel
        {
            get { return $"{this.DisplayName} - [{this.EmailAddress}]"; }
        }  

        public User(SecUser user)
        {
            try
            {
                this.UserId = user.UserId;
                this.EmailAddress = user.EmailAddress;
                this.DisplayName = user.DisplayName;
                this.OrgId = user.OrgId;
                this.ManagerId = user.ManagerId;
                this.Organization = new Organization(user.Org);
                this.OrganizationName = Organization.Label;
                this.UserRoles = new List<UserRole>();
                this.CreatedAt = user.CreatedAt;
                this.DeletedAt = user.DeletedAt;
                foreach(SecUserRole userRole in user.SecUserRole)
                { 
                    if(userRole.DeletedAt == null && userRole.Role.DeletedAt == null)
                        this.UserRoles.Add(new UserRole(userRole));
                }                   
            }
            catch(Exception)
            {

            }
        }

        public User(SecUser user, bool enableUserSecurityLoading)
        {
            try
            {
                this.UserId = user.UserId;
                this.EmailAddress = user.EmailAddress;
                this.DisplayName = user.DisplayName;
                this.OrgId = user.OrgId;
                this.ManagerId = user.ManagerId;
                this.CreatedAt = user.CreatedAt;
                this.DeletedAt = user.DeletedAt;
                if(this.ManagerId.HasValue && user.Manager != null)
                {
                    this.ManagerName = user.Manager.DisplayName;
                }                
                this.Organization = new Organization(user.Org);
                this.OrganizationName = Organization.Label;
                this.UserRoles = new List<UserRole>();
                if(enableUserSecurityLoading)
                {
                    foreach(SecUserRole userRole in user.SecUserRole)
                    {
                        if(userRole.DeletedAt == null && userRole.Role.DeletedAt == null)
                            this.UserRoles.Add(new UserRole(userRole));
                    }

                }                
                
            }
            catch (System.Exception)
            {
            }


        }


        public User(SecUser user, bool enableUserSecurityLoading, bool enableOrganizationLoading)
        {
            try
            {
                this.UserId = user.UserId;
                this.EmailAddress = user.EmailAddress;
                this.DisplayName = user.DisplayName;
                this.OrgId = user.OrgId;
                this.ManagerId = user.ManagerId;
                this.CreatedAt = user.CreatedAt;
                this.DeletedAt = user.DeletedAt;
                if(this.ManagerId.HasValue && user.Manager != null)
                {
                    this.ManagerName = user.Manager.DisplayName;
                }
                if(enableOrganizationLoading)
                {
                    this.Organization = new Organization(user.Org);
                    this.OrganizationName = Organization.Label;

                }
                this.UserRoles = new List<UserRole>();
                if(enableUserSecurityLoading)
                {
                    foreach(SecUserRole userRole in user.SecUserRole)
                    {
                        if(userRole.DeletedAt == null && userRole.Role.DeletedAt == null)
                            this.UserRoles.Add(new UserRole(userRole));
                    }

                }                
                
            }
            catch (System.Exception)
            {
            }

        }        


        public User()
        {
            this.UserRoles = new List<UserRole>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("User = {");
            sb.Append("UserId: ");
            sb.Append(this.UserId);
            sb.Append(", ManagerId: ");
            sb.Append(this.ManagerId);
            sb.Append(", OrgId: ");
            sb.Append(this.OrgId);
            sb.Append(", EmailAddress: ");
            sb.Append(this.EmailAddress);
            sb.Append(", DisplayName: ");
            sb.Append(this.DisplayName);
            sb.Append(", DisplayLabel: ");
            sb.Append(this.DisplayLabel);
            sb.Append("}");
            return sb.ToString();
        }

        internal SecUser SecUser()
        {
            SecUser secUser = new SecUser();
            secUser.CreatedAt = this.CreatedAt;
            secUser.DeletedAt = this.DeletedAt;
            secUser.DisplayName = this.DisplayName;
            secUser.EmailAddress = this.EmailAddress;
            secUser.ManagerId = this.ManagerId;
            secUser.OrgId = this.OrgId;
            secUser.UpdatedAt = this.UpdatedAt;
            secUser.UserId = this.UserId;

            foreach(var userrole in UserRoles)
            {
                SecUserRole secUserRole = new SecUserRole();
                secUserRole.RoleId = userrole.RoleId;
                secUserRole.UserId = this.UserId;
                secUserRole.CreatedAt = DateTime.Now;
                secUser.SecUserRole.Add(secUserRole);
            }
            return secUser;
        }

    }
}