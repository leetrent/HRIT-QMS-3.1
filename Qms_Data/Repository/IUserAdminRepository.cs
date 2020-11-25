using System;
using QmsCore.Model;
using System.Linq;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public interface IUserAdminRepository
    {
        IQueryable<SecPermission> RetrieveSecPermissions();
        IList<SecRolePermission> RetrieveSecRolePermissions();

        SecRole RetrieveSecRole(int roleId);
      
        IQueryable<SecRole> RetrieveSecRoles();

        SecPermission RetrieveSecPermission(int permissionId);
        int SavePermission(SecPermission secPermission);
        int CreatePermission(SecPermission secPermission);
        int UpdatePermission(SecPermission secPermission);
        int DeletePermission(SecPermission secPermission);

        int CreateRole(SecRole secRole);
        int UpdateRole(SecRole secRole);
        int CreateSecRolePermission(SecRolePermission secRolePermission);

        IList<SecUserRole> RetrieveSecUserRoles();

        SecUser RetrieveSecUser(int userId);

        int CreateUser(SecUser secUser);
        int UpdateUser(SecUser secUser);

        int CreateOrg(SecOrg secOrg);
    }
}