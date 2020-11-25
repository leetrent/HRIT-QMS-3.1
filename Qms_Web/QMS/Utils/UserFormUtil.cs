using System;
using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.ViewModels;

namespace QMS.Utils
{
    public class UserFormUtil : IUserFormUtil
    {
        private readonly IRoleService _roleService;

        public UserFormUtil(IRoleService roleSvc)
        {
            _roleService = roleSvc;
        }

        public User MapToUIModelOnCreate(UserFormViewModel userVM, string[] selectedRoleIdsForUser)
        {
            User userDB = new User();

            if (userVM.UserId > 0)
            {
                userDB.UserId = userVM.UserId;
            }

            if ( userVM.ManagerId.HasValue)
            {
                userDB.ManagerId = userVM.ManagerId;
            }
            userDB.OrgId        = userVM.OrgId;
            userDB.EmailAddress = userVM.EmailAddress;
            userDB.DisplayName  = userVM.DisplayName;

            foreach (string selectedRoleIdForUser in selectedRoleIdsForUser)
            {
                UserRole userRole = new UserRole();
                userRole.RoleId = Int32.Parse(selectedRoleIdForUser);
                userDB.UserRoles.Add(userRole);
            }

            return userDB;
        }

        
        public UserFormViewModel MapToViewModel(User userDB)
        {
            UserFormViewModel userFormVM = new UserFormViewModel();

            userFormVM.UserId       = userDB.UserId;
            userFormVM.ManagerId    = userDB.ManagerId;
            userFormVM.OrgId        = userDB.OrgId;
            userFormVM.EmailAddress = userDB.EmailAddress;
            userFormVM.DisplayName  = userDB.DisplayName;

            HashSet<int> selectedRoleIdIntSetForUser = new HashSet<int>();

            foreach (UserRole userRoleDB in userDB.UserRoles)
            {
                userFormVM.Roles.Add(this.createUARoleViewModel(userRoleDB.Role));
                selectedRoleIdIntSetForUser.Add(userRoleDB.Role.RoleId);
            }

            List<Role> allActiveDbRoles = _roleService.RetrieveActiveRoles();
            foreach (Role activeDbRole in allActiveDbRoles)
            {
                userFormVM.CheckboxRoles.Add(this.createUARoleViewModel(activeDbRole));
            }

            foreach (UARoleViewModel checkboxRole in userFormVM.CheckboxRoles)
            {
                checkboxRole.Selected = selectedRoleIdIntSetForUser.Contains(checkboxRole.RoleId);
            }

            return userFormVM;
        }
        
        /*
        public UserFormViewModel MapToViewModel(User dbUser)
        {
            UserFormViewModel vmUser = this.createUserFormViewModel(dbUser);

            foreach (UserRole dbUserRole in dbUser.UserRoles)
            {
                vmUser.Roles.Add(this.createUARoleViewModel(dbUserRole.Role));
            }

            List<Role> allActiveDbRoles = _roleService.RetrieveActiveRoles();
            foreach (Role activeDbRole in allActiveDbRoles)
            {
                vmUser.CheckboxRoles.Add(this.createUARoleViewModel(activeDbRole));
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
        */



        public void PopulateCheckboxRolesForCreateUser(UserFormViewModel newUser)
        {
            List<Role> allActiveDbRoles = _roleService.RetrieveActiveRoles();
            foreach (Role activeDbRole in allActiveDbRoles)
            {
                newUser.CheckboxRoles.Add(this.createUARoleViewModel(activeDbRole));
            }
        }

        public void PopulateCheckboxRolesForUser(UserFormViewModel userFormVM, string[] selectedRoleIdStringArrayForUser)
        {
            // CONVERT STRING ARRAY TO INTEGER ARRAY
            int[] selectedRoleIdIntArrayForUser = Array.ConvertAll(selectedRoleIdStringArrayForUser, int.Parse);

            // CONVERT INTEGER ARRAY TO SET OF INTEGERS
            HashSet<int> selectedRoleIdIntSetForUser = new HashSet<int>(selectedRoleIdIntArrayForUser);

            // For every occurrence of CheckboxRole, set the 'Selected" property to true 
            // where the role id is containted in selectedRoleIdIntSetForUser.
            foreach (UARoleViewModel checkboxRole in userFormVM.CheckboxRoles)
            {
                checkboxRole.Selected = selectedRoleIdIntSetForUser.Contains(checkboxRole.RoleId);
            }
        }


        private UserFormViewModel createUserFormViewModel(User dbUser)
        {
            UserFormViewModel vmUser = new UserFormViewModel();

            vmUser.UserId       = dbUser.UserId;
            vmUser.ManagerId    = dbUser.ManagerId;
            vmUser.OrgId        = dbUser.OrgId;
            vmUser.EmailAddress = dbUser.EmailAddress;
            vmUser.DisplayName  = dbUser.DisplayName;

            return vmUser;
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