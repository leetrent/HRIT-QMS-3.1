using System;
using QmsCore.Model;
using System.Linq;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public interface ISecPermissionRepository
    {
        IQueryable<SecPermission> RetrieveAllSecPermissions();
        IQueryable<SecPermission> RetrieveActiveSecPermissions();
        IQueryable<SecPermission> RetrieveInactiveSecPermissions();
        SecPermission RetrieveSecPermission(int permissionId);
        int CreatePermission(SecPermission secPermission);
        int UpdatePermission(SecPermission secPermission);
    }
}