using System.Collections.Generic;
using QmsCore.UIModel;
using QMS.Constants;

namespace QMS.ViewModels
{
    public class RoleViewModel
    {
        public bool     ShowAlert       { get; set; } = false;
        public string   UserAdminModule { get; set; }
        public string   AlertMessage    { get; set; }
        public string   AlertType       { get; set; }

        public List<Role>       ActiveRoles { get; set; }
        public List<Role>       InactiveRoles { get; set; }
        public List<Permission> Permissions { get; set; }

        public string RoleNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string ActiveRoleNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string InactiveRoleNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;

        public string RoleTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string ActiveRoleTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string InactiveRoleTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;

        public string AlertCssClass
        {
            get
            {
                return (UserAdminConstants.AlertTypeConstants.SUCCESS.Equals(this.AlertType)) ? (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_SUCCESS)
                                                                                                : (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_FAILURE);
            }
        }

        public int JustEditedRoleId { get; set; } = 0;
    }
}