using QmsCore.UIModel;

namespace QmsCore.Lib
{
    public class UserUtil
    {

       public static bool UserHasPermission(User user, string PermissionCode)
        {
            bool retval = false;
            foreach(var userrole in user.UserRoles)
            {
                foreach(var permission in userrole.Role.Permissions)
                {
                    if(permission.PermissionCode == PermissionCode)
                    {
                        retval = true;
                        break;
                    }
                }
            }
            return retval;
        }

        public static bool UserHasRole(User user, string RoleCode)
        {
            bool retval = false;
            foreach(var userrole in user.UserRoles)
            {
                if(userrole.Role.RoleCode == RoleCode)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }        
    }
}//end namespace