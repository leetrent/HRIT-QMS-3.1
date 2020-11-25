using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IPermissionService
    {
        List<Permission> RetrieveAllPermissions();
        List<Permission> RetrieveActivePermissions();
        List<Permission> RetrieveInactivePermissions();
        Permission RetrievePermission(int permissionId);
        int CreatePermission(string permissionCode, string permissionLabel, User user);
        int UpdatePermission(int permissionId, string perissionLabel, User user);
        int DeactivatePermission(int permissionId, User user);
        int ReactivatePermission(int permissionId, User user);
    }
}