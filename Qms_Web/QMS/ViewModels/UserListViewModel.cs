using System.Collections.Generic;
using QmsCore.UIModel;
using QMS.Constants;

namespace QMS.ViewModels
{
    public class UserListViewModel
    {
        public bool   ShowAlert { get; set; } = false;
        public string AlertType { get; set; }
        public string AlertMessage { get; set; }
        public string UserAdminModule { get; set; }

        public string UserNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string ActiveUserNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string InactiveUserNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;

        public string UserTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string ActiveUserTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;
        public string InactiveUserTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;

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