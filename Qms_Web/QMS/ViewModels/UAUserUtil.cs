using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.ViewModels;

namespace QMS.Utils
{
    public class UAUserUtil : IUAUserUtil
    {
        private readonly IRoleService _roleService;

        public UAUserUtil(IRoleService roleSvc)
        {
            _roleService = roleSvc;
        }

        public UAUserViewModel MapToViewModel(User dbUser)
        {
            UAUserViewModel vmUser = this.createUAUserViewModel(dbUser);

            foreach (UserRole dbUserRole in dbUser.UserRoles)
            {
                vmUser.Roles.Add(this.createUARoleViewModel(dbUserRole));
            }

            List<Role> allActiveDbRoles = _roleService.RetrieveActiveRoles();
            foreach (Role activeDbRole in allActiveDbRoles)
            {
                vmUser.CheckboxRoles.Add( this.createUARoleViewModel(activeDbRole) );
            }

            List<int> rolesForUserIdList = new List<int>();
            foreach (UARoleViewModel vmRole in vmUser.Roles)
            {
                rolesForUserIdList.Add(vmRole.RoleId);
            }

            HashSet<int> rolesForUserIdSet = new HashSet<int>(rolesForUserIdList);

            foreach (UARoleViewModel checkboxRole in vmUser.CheckboxRoles)
            {
                checkboxRole.Selected = rolesForUserIdSet.Contains(checkboxRole.RoleId);
            }

            return vmUser;
        }

        public void PopulateCheckboxRolesForCreateUser(UAUserViewModel newUser)
        {
            List<Role> allActiveDbRoles = _roleService.RetrieveActiveRoles();
            foreach (Role activeDbRole in allActiveDbRoles)
            {
                newUser.CheckboxRoles.Add(this.createUARoleViewModel(activeDbRole));
            }
        }

        private UAUserViewModel createUAUserViewModel(User dbUser)
        {
            UAUserViewModel vmUser = new UAUserViewModel();

            vmUser.UserId       = dbUser.UserId;
            vmUser.ManagerId    = dbUser.ManagerId;
            vmUser.OrgId        = dbUser.OrgId;
            vmUser.EmailAddress = dbUser.EmailAddress;
            vmUser.DisplayName  = dbUser.DisplayName;

            return vmUser;
        }

        private UARoleViewModel createUARoleViewModel(UserRole dbUserRole)
        {
            UARoleViewModel vmRole = new UARoleViewModel();

            vmRole.RoleId       = dbUserRole.Role.RoleId;
            vmRole.RoleCode     = dbUserRole.Role.RoleCode;
            vmRole.RoleLabel    = dbUserRole.Role.RoleLabel;

            return vmRole;
        }

        private UARoleViewModel createUARoleViewModel(Role dbRole)
        {
            UARoleViewModel vmRole = new UARoleViewModel();

            vmRole.RoleId       = dbRole.RoleId;
            vmRole.RoleCode     = dbRole.RoleCode;
            vmRole.RoleLabel    = dbRole.RoleLabel;

            return vmRole;
        }
    }
}