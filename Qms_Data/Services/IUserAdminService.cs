using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IUserAdminService
    {
        List<Permission> RetrieveAllPermissions();
        List<Role> RetrieveAllRoles();

//        List<OrganizationalRole> RetrieveAllOrganizationRoles();

        Role RetrieveRole(int roleId);

        List<Role> RetrieveRoles();

        int SavePermission(string permissionCode, string permissionLabel);
        int CreatePermission(string permissionCode, string permissionLabel);
        Permission RetrievePermission(int permissionId);
        int UpdatePermission(int permissionId, string permissionCode, string perissionLabel);
        int DeletePermission(Permission permission);
        int CreateRole(Role role);
        int UpdateRole(Role role);

        int CreateUser(User user);
        int CreateOrg(Organization org);

    }
}