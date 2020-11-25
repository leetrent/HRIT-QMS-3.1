using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QMS.Helpers
{
	public class UserAdminHelper : IUserAdminHelper
    {
		private readonly IPermissionService _permissionService;

		public UserAdminHelper(IPermissionService permSvc)
		{
			_permissionService = permSvc;
		}

        public List<Permission> BuildSelectedPermissions(string[] selectedPermissionIdStrings)
        {
            List<Permission> allPermissions         = _permissionService.RetrieveAllPermissions();
            List<Permission> selectedPermissions    = new List<Permission>();
            int[] selectedPermissionIds             = Array.ConvertAll(selectedPermissionIdStrings, int.Parse);

            foreach (int selectedPermissionId in selectedPermissionIds)
            {
                foreach(Permission availablePermission in allPermissions)
                {
                    if (availablePermission.PermissionId == selectedPermissionId)
                    {
                        selectedPermissions.Add(availablePermission);
                    }
                }
            }
            return selectedPermissions;
        }

        public void AddSelectedPermissionsToRole(string[] selectedPermissionIdStrings, Role role)
        {
            List<Permission> allPermissions = _permissionService.RetrieveAllPermissions();
            int[] selectedPermissionIds = Array.ConvertAll(selectedPermissionIdStrings, int.Parse);

            foreach (int selectedPermissionId in selectedPermissionIds)
            {
                foreach (Permission availablePermission in allPermissions)
                {
                    if (availablePermission.PermissionId == selectedPermissionId)
                    {
                        role.Permissions.Add(availablePermission);
                    }
                }
            }
        }

        public Role CreateNewRole(string roleCode, string roleLabel, string[] selectedPermissionIdStrings, string roleId = null)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserAdminHelper][CreateNewRole] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(roleId)..: '{roleId}'");
            Console.WriteLine(logSnippet + $"(roleCode): '{roleCode}'");
            Console.WriteLine(logSnippet + $"(roleCode): '{roleCode}'");
            Console.WriteLine(logSnippet + $"(roleCode): '{roleCode}'");
            Console.WriteLine(logSnippet + $"(selectedPermissionIdStrings == null): '{selectedPermissionIdStrings == null}'");

            foreach (string selectedPermissionId in selectedPermissionIdStrings)
            {
                Console.WriteLine(logSnippet + $"(selectedPermissionId): '{selectedPermissionId}'");
            }

            int roleIdForUpdate = 0;
            try
            {
                roleIdForUpdate = Int32.Parse(roleId);
            }
            catch (Exception)
            {
                roleIdForUpdate = 0;
            }

            Role role = new Role
            {
                RoleId   = roleIdForUpdate,
                RoleCode = roleCode,
                RoleLabel = roleLabel
            };

            List<Permission> allPermissions = _permissionService.RetrieveAllPermissions();
            int[] selectedPermissionIds = Array.ConvertAll(selectedPermissionIdStrings, int.Parse);

            foreach (int selectedPermissionId in selectedPermissionIds)
            {
                foreach (Permission availablePermission in allPermissions)
                {
                    if (availablePermission.PermissionId == selectedPermissionId)
                    {
                        role.Permissions.Add(availablePermission);
                    }
                }
            }

            return role;
        }

        public void ProcessPermissionCheckboxesForAllRoles(List<Role> allRoles, List<Permission> allPermissions)
        {
            foreach (Role role in allRoles)
            {
                this.ProcessPermissionCheckboxesForSingleRole(role, allPermissions);
            }
        }

        private void ProcessPermissionCheckboxesForSingleRole(Role role, List<Permission> allAvailablePermissions)
        {
            
            /*
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserAdminHelper][ProcessPermissionCheckboxesForSingleRole] => ")
                    .ToString();       
  
           if ( role.RoleCode.Equals("CA_VIEWER"))
           {
                Console.WriteLine(logSnippet + $"(role.RoleId)...................: '{role.RoleId}'");
                Console.WriteLine(logSnippet + $"(role.RoleCode).................: '{role.RoleCode}'");
                Console.WriteLine(logSnippet + $"(role.RoleLabel)................: '{role.RoleLabel}'");
                Console.WriteLine(logSnippet + $"(role.CheckboxPermissions.Count): '{role.CheckboxPermissions.Count}'");
                Console.WriteLine(logSnippet + $"(role.Permissions.Count)........: '{role.Permissions.Count}'");
           }
           */

            role.CheckboxPermissions = this.DeepCopy(allAvailablePermissions);
            List<int> permissionsForRoleIdList = new List<int>();

            if (role != null && role.Permissions != null)
            {
                foreach (Permission permission in role.Permissions)
                {
                    permissionsForRoleIdList.Add(permission.PermissionId);
                }
            }
            HashSet<int> permissionsForRolesIdSet = new HashSet<int>(permissionsForRoleIdList);

            /*
            if ( role.RoleCode.Equals("CA_VIEWER"))
            {
                Console.WriteLine(logSnippet + $"(permissionsForRoleIdList.Count): '{permissionsForRoleIdList.Count}'");
                Console.WriteLine(logSnippet + $"(permissionsForRolesIdSet.Count): '{permissionsForRolesIdSet.Count}'");
            }
            */

            foreach (Permission checkboxPermission in role.CheckboxPermissions)
            {
                checkboxPermission.Selected = permissionsForRolesIdSet.Contains(checkboxPermission.PermissionId);
            }

            /*
            if ( role.RoleCode.Equals("CA_VIEWER"))
            {
                foreach (Permission checkboxPermission in role.CheckboxPermissions)
                {
                    Console.WriteLine(logSnippet + $"(checkboxPermission.PermissionId)...: '{checkboxPermission.PermissionId}'");
                    Console.WriteLine(logSnippet + $"(checkboxPermission.PermissionCode).: '{checkboxPermission.PermissionCode}'");
                    Console.WriteLine(logSnippet + $"(checkboxPermission.PermissionLabel): '{checkboxPermission.PermissionLabel}'");
                    Console.WriteLine(logSnippet + $"(checkboxPermission.Selected).......: '{checkboxPermission.Selected}'");
                }
            }
            */
        }

        private List<Permission> DeepCopy(List<Permission> sourcePermissions)
        {
            List<Permission> copiedPermissions = new List<Permission>();
            foreach (Permission sourcePermission in sourcePermissions)
            {
                copiedPermissions.Add(sourcePermission.DeepCopy());
            }
            return copiedPermissions;
        }
	}
}