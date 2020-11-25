using System.Collections.Generic;
using QmsCore.UIModel;
using QMS.Constants;

namespace QMS.ViewModels
{
    public class PermissionViewModel
    {
        public bool   ShowAlert { get; set; } = false;
        public string AlertType { get; set; }
        public string AlertMessage { get; set; }
        public string UserAdminModule { get; set; }

        public string PermissionNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string ActivePermissionNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string InactivePermissionNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;

        public string PermissionTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string ActivePermissionTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string InactivePermissionTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;

        public string AlertCssClass
        {
            get
            {
                return (UserAdminConstants.AlertTypeConstants.SUCCESS.Equals(this.AlertType)) ? (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_SUCCESS)
                                                                                                : (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_FAILURE);
            }
        }
    }
}