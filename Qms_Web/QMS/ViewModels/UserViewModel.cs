using System.Text;
using System.Collections.Generic;
using QmsCore.UIModel;

namespace QMS.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int? ManagerId { get; set; }
        public int OrgId { get; set; }
        public string OrgLabel { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public string DisplayLabel
        {
            get { return $"{this.DisplayName} - [{this.EmailAddress}]"; }
        } 

        public List<Role> Roles { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<string> RoleLabels { get; set; }

        //////////////////////////////////////////////////////////////////////
        // QMS Roles
        //////////////////////////////////////////////////////////////////////
        public bool IsSysAdmin { get; set; } = false;

        //////////////////////////////////////////////////////////////////////
        // CORRECTIVE ACTION PERMISSIONS
        //////////////////////////////////////////////////////////////////////
        //public bool CanViewCorrectiveActionList  { get; set; } = false;
        //public bool CanViewAllCorrectiveActionList { get; set; } = false;
        public bool CanCreateCorrectiveAction  { get; set; } = false;
        public bool CanEditCorrectiveAction  { get; set; } = false;
        public bool CanViewNotifications { get; set; } = false;
        public bool CanAssignTasks  { get; set; } = false;
        public bool CanCommentOnTask { get; set; } = false;

        public bool CanViewAllCorrectiveActions             { get; set; } = false;
        public bool CanViewAllArchivedCorrectiveActions     { get; set; } = false;
        public bool CanViewCorrectiveActionsForUser         { get; set; } = false;
        public bool CanViewCorrectiveActionForOrg           { get; set; } = false;
        public bool CanViewArchivedCorrectiveActionsForUser { get; set; } = false;
        public bool CanViewArchivedCorrectiveActionForOrg   { get; set; } = false;


        //////////////////////////////////////////////////////////////////////
        // USER ADMIN PERMISSIONS
        ////////////////////////////////////////////////////////////////////// 

        public bool CanCreateUser       { get; set; } = false;
        public bool CanRetrieveUser     { get; set; } = false;
        public bool CanUpdateUser       { get; set; } = false;
        public bool CanDeactivateUser   { get; set; } = false;
        public bool CanReactivateUser   { get; set; } = false;

        public bool CanCreateRole       { get; set; } = false;
        public bool CanRetrieveRole     { get; set; } = false;
        public bool CanUpdateRole       { get; set; } = false;
        public bool CanDeactivateRole   { get; set; } = false;
        public bool CanReactivateRole   { get; set; } = false;

        public bool CanCreatePermission     { get; set; } = false;
        public bool CanRetrievePermission   { get; set; } = false;
        public bool CanUpdatePermission     { get; set; } = false;
        public bool CanDeactivatePermission { get; set; } = false;
        public bool CanReactivatePermission { get; set; } = false;
    

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UserViewModel = {");
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

    }
}