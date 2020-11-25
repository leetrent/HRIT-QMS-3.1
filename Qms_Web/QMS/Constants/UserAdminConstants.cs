namespace QMS.Constants
{
    public class UserAdminConstants
    {
        public static readonly string USER_ADMIN_VIEW_MODEL = "USER_ADMIN_VIEW_MODEL";
        public static readonly string PERMISSION_VIEW_MODEL = "PERMISSION_VIEW_MODEL";
        public static readonly string ROLE_VIEW_MODEL       = "ROLE_VIEW_MODEL";
        public static readonly string USER_LIST_VIEW_MODEL  = "USER_LIST_VIEW_MODEL";
        public static readonly string USER_FORM_VIEW_MODEL  = "USER_FORM_VIEW_MODEL";
        public static readonly string UA_USER_VIEW_MODEL    = "UA_USER_VIEW_MODEL";

        public class AlertTypeConstants
        {
            public static readonly string SUCCESS = "SUCCESS";
            public static readonly string FAILURE = "FAILURE";
        }

        public class UserAdminModuleConstants
        {
            public static readonly string ACTIVE_USER           = "ACTIVE_USER";
            public static readonly string INACTIVE_USER         = "INACTIVE_USER";
            public static readonly string ACTIVE_ROLE           = "ACTIVE_ROLE";
            public static readonly string INACTIVE_ROLE         = "INACTIVE_ROLE";
            public static readonly string ACTIVE_PERMISSION     = "ACTIVE_PERMISSION";
            public static readonly string INACTIVE_PERMISSION   = "INACTIVE_PERMISSION";
            public static readonly string ACTIVE_ORGANIZATION   = "ACTIVE_ORGANIZATION";
            public static readonly string INACTIVE_ORGANIZATION = "INACTIVE_ORGANIZATION";
        }

        public class UserAdminCssConstants
        {
            public static readonly string DEFAULT_NAVITEM_NAVLINK_VALUE    = "nav-item nav-link";
            public static readonly string ACTIVE_NAVITEM_NAVLINK_VALUE     = "nav-item nav-link active";

            public static readonly string DEFAULT_TABPANE_FADE_VALUE    = "tab-pane fade";
            public static readonly string ACTIVE_TABPANE_FADE_VALUE     = "tab-pane fade show active";

            public static readonly string ALERT_CSS_SUCCESS = "alert alert-success alert-success alert-dismissible fade show mt-3";
            public static readonly string ALERT_CSS_FAILURE = "alert alert-danger alert-failure alert-dismissible fade show mt-3";
        }
    }
}