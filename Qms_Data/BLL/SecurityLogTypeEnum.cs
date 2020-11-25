using System;

namespace QmsCore.Model
{
    public class SecurityLogTypeEnum
    {
        public static readonly string PERM_CREATED = "PERM_CREATED";
        public static readonly string PERM_UPDATED = "PERM_UPDATED";
        public static readonly string PERM_DEACTIVATED = "PERM_DEACTIVATED";
        public static readonly string PERM_REACTIVATED = "PERM_REACTIVATED";
        public static readonly string PERM_ADDED_TO_ROLE = "PERM_ADDED_TO_ROLE";
        public static readonly string PERM_REMOVED_FROM_ROLE = "PERM_REMOVED_FROM_ROLE";
        public static readonly string ROLE_CREATED = "ROLE_CREATED";
        public static readonly string ROLE_UPDATED = "ROLE_UPDATED";
        public static readonly string ROLE_DEACTIVATED = "ROLE_DEACTIVATED";
        public static readonly string ROLE_REACTIVATED = "ROLE_REACTIVATED";
        public static readonly string USER_CREATED = "USER_CREATED";
        public static readonly string USER_UPDATED = "USER_UPDATED";
        public static readonly string USER_DEACTIVATED = "USER_DEACTIVATED";
        public static readonly string USER_REACTIVATED = "USER_REACTIVATED";
        public static readonly string USER_ASSIGNED_ROLE = "USER_ASSIGNED_ROLE";
        public static readonly string USER_UNASSIGNED_ROLE = "USER_UNASSIGNED_ROLE";
        public static readonly string USER_LOGIN_SUCCESS = "USER_LOGIN_SUCCESS";
        public static readonly string USER_LOGIN_FAIL = "USER_LOGIN_FAIL";
        public static readonly string USER_LOGIN_NAVIGATE = "USER_LOGIN_NAVIGATE";
        public static readonly string USER_LOGIN_NAVIGATE_TO_PAGE = "USER_LOGIN_NAVIGATE_TO_PAGE";
        
        public static readonly string ORG_CREATED = "ORG_CREATED";
        public static readonly string ORG_UPDATED = "ORG_UPDATED";
        public static readonly string ORG_DEACTIVATED = "ORG_DEACTIVATED";
        public static readonly string ORG_REACTIVATED = "ORG_REACTIVATED";

    }//end class
}//end namespace