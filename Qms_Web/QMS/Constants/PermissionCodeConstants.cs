namespace QMS.Constants
{
    public class PermissionCodeConstants
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // CORRECTIVE ACTION PERMISSIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //public static readonly string VIEW_CORRECTIVE_ACTION_LIST = "VIEW_CORRECTIVE_ACTION_LIST";
        //public static readonly string VIEW_ALL_CORRECTIVE_ACTION_LIST = "VIEW_ALL_CORRECTIVE_ACTION_LIST";
        public static readonly string CREATE_CORRECTIVE_ACTION = "CREATE_CORRECTIVE_ACTION";
        public static readonly string EDIT_CORRECTIVE_ACTION = "EDIT_CORRECTIVE_ACTION";
        public static readonly string ASSIGN_TASKS = "ASSIGN_TASKS";
        public static readonly string COMMENT_ON_TASK = "COMMENT_ON_TASK";
        public static readonly string REVIEW_NOTIFICATIONS = "REVIEW_NOTIFICATIONS";

        public static readonly string VIEW_ALL_CORRECTIVE_ACTIONS                   = "VIEW_ALL_CORRECTIVE_ACTIONS";
        public static readonly string VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS          = "VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS";

        public static readonly string VIEW_CORRECTIVE_ACTIONS_FOR_USER              = "VIEW_CORRECTIVE_ACTIONS_FOR_USER";
        public static readonly string VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_USER     = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_USER";

        public static readonly string VIEW_CORRECTIVE_ACTIONS_FOR_ORG               = "VIEW_CORRECTIVE_ACTIONS_FOR_ORG";      
        public static readonly string VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_ORG      = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_ORG";

        public static readonly string VIEW_CORRECTIVE_ACTIONS_FOR_POID              = "VIEW_CORRECTIVE_ACTIONS_FOR_POID";
        public static readonly string VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_POID     = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_POID";


        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // USER ADMIN PERMISSIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly string CREATE_USER       = "CREATE_USER";
        public static readonly string RETRIEVE_USER     = "RETRIEVE_USER";
        public static readonly string UPDATE_USER       = "UPDATE_USER";
        public static readonly string DEACTIVATE_USER   = "DEACTIVATE_USER";
        public static readonly string REACTIVATE_USER   = "REACTIVATE_USER";

        public static readonly string CREATE_ROLE       = "CREATE_ROLE";
        public static readonly string RETRIEVE_ROLE     = "RETRIEVE_ROLE";
        public static readonly string UPDATE_ROLE       = "UPDATE_ROLE";
        public static readonly string DEACTIVATE_ROLE   = "DEACTIVATE_ROLE";
        public static readonly string REACTIVATE_ROLE   = "REACTIVATE_ROLE";
        
        public static readonly string CREATE_PERMISSION     = "CREATE_PERMISSION";
        public static readonly string RETRIEVE_PERMISSION   = "RETRIEVE_PERMISSION";
        public static readonly string UPDATE_PERMISSION     = "UPDATE_PERMISSION";
        public static readonly string DEACTIVATE_PERMISSION = "DEACTIVATE_PERMISSION";
        public static readonly string REACTIVATE_PERMISSION = "REACTIVATE_PERMISSION";

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // EHRI PERMISSIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly string VIEW_ALL_EHRI_ERRORS                  = "VIEW_ALL_EHRI_ERRORS";
        public static readonly string VIEW_ALL_ARCHIVED_EHRI_ERRORS         = "VIEW_ALL_ARCHIVED_EHRI_ERRORS";
        public static readonly string VIEW_EHRI_ERRORS_FOR_USER             = "VIEW_EHRI_ERRORS_FOR_USER";
        public static readonly string VIEW_EHRI_ERRORS_FOR_ORG              = "VIEW_EHRI_ERRORS_FOR_ORG";
        public static readonly string VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER    = "VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER";
        public static readonly string VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG     = "VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG";
    }
}