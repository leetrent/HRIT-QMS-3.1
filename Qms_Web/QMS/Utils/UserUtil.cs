using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.UIModel;
using QMS.ViewModels;
using QMS.Constants;

namespace QMS.Utils
{
    public static class UserUtil
    {
        public static UserViewModel MapToViewModel(User entity)
        {
            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserUtil][MapToViewModel] => ")
                                .ToString();

            UserViewModel vm = new UserViewModel {
                UserId = entity.UserId,
                OrgId = entity.OrgId.Value,
                OrgLabel = entity.Organization.OrgLabel,
                EmailAddress = entity.EmailAddress,
            };

   
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // User Roles
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UserUtil.ExtractRolesAndRoleLabels(vm, entity);
            vm.IsSysAdmin = UserUtil.UserHasThisRole(entity, RoleCodeConstants.SYS_ADMIN);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // User Permission
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            vm.Permissions = UserUtil.ExtractPermissions(entity);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CORRECTIVE ACTION PERMISSIONS
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //vm.CanViewCorrectiveActionList      = UserUtil.UserHasThisPermission(role, PermissionCodeConstants.VIEW_CORRECTIVE_ACTION_LIST);
            //vm.CanViewAllCorrectiveActionList   = UserUtil.UserHasThisPermission(role, PermissionCodeConstants.VIEW_ALL_CORRECTIVE_ACTION_LIST);
            vm.CanCreateCorrectiveAction        = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.CREATE_CORRECTIVE_ACTION);
            vm.CanEditCorrectiveAction          = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.EDIT_CORRECTIVE_ACTION);
            vm.CanAssignTasks                   = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.ASSIGN_TASKS);
            vm.CanCommentOnTask                 = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.COMMENT_ON_TASK);
            vm.CanViewNotifications             = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.REVIEW_NOTIFICATIONS);

            vm.CanViewAllCorrectiveActions              = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_ALL_CORRECTIVE_ACTIONS);
            vm.CanViewAllArchivedCorrectiveActions      = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS);
            vm.CanViewCorrectiveActionsForUser          = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_CORRECTIVE_ACTIONS_FOR_USER);
            vm.CanViewCorrectiveActionForOrg            = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_CORRECTIVE_ACTIONS_FOR_ORG);
            vm.CanViewArchivedCorrectiveActionsForUser  = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_USER);
            vm.CanViewArchivedCorrectiveActionForOrg    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_ORG);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // USER ADMIN PERMISSIONS
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            vm.CanCreateUser        = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.CREATE_USER);
            vm.CanRetrieveUser      = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.RETRIEVE_USER);
            vm.CanUpdateUser        = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.UPDATE_USER);          
            vm.CanDeactivateUser    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.DEACTIVATE_USER);
            vm.CanReactivateUser    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.REACTIVATE_USER);            

            vm.CanCreateRole        = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.CREATE_ROLE);
            vm.CanRetrieveRole      = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.RETRIEVE_ROLE);
            vm.CanUpdateRole        = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.UPDATE_ROLE);          
            vm.CanDeactivateRole    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.DEACTIVATE_ROLE);
            vm.CanReactivateRole    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.REACTIVATE_ROLE);

            vm.CanCreatePermission      = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.CREATE_PERMISSION);
            vm.CanRetrievePermission    = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.RETRIEVE_PERMISSION);
            vm.CanUpdatePermission      = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.UPDATE_PERMISSION);
            vm.CanDeactivatePermission  = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.DEACTIVATE_PERMISSION);
            vm.CanReactivatePermission  = UserUtil.UserHasThisPermission(entity, PermissionCodeConstants.REACTIVATE_PERMISSION);

            Console.WriteLine(logSnippet + $"BEGIN (UserViewModel.ToString()):");
            Console.WriteLine(vm);
            Console.WriteLine(logSnippet + $":END (UserViewModel.ToString())");

            return vm;
        }

        private static void ExtractRolesAndRoleLabels(UserViewModel vm, User entity)
        {
            vm.Roles = new List<Role>();
            vm.RoleLabels = new List<string>();

            foreach (UserRole userRole in entity.UserRoles)
            {
                vm.Roles.Add(userRole.Role);
                vm.RoleLabels.Add(userRole.Role.RoleLabel);
            }
        }

        public static bool UserHasThisRole(User user, string roleCode)
        {
            foreach (UserRole userRole in user.UserRoles)
            {
                if ( userRole.Role.RoleCode.Equals(roleCode) )
                {
                    return true;
                }
            }
            return false;
        }

        private static List<Permission> ExtractPermissions(User user)
        {
            List<Permission> permissions = new List<Permission>();
            foreach (UserRole userRole in user.UserRoles)
            {
                foreach (Permission permission in userRole.Role.Permissions)
                {
                    permissions.Add(permission);
                }
            }
            return permissions;
        }

        public static bool UserHasThisPermission(User user, string permissionCode)
        {
            foreach (UserRole userRole in user.UserRoles)
            {
                foreach (Permission permission in userRole.Role.Permissions)
                {
                    if (permission.PermissionCode.Equals(permissionCode) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}