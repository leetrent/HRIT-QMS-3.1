using System;
using QmsCore.Model;
using System.Linq;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public interface ISecRoleRepository
    {
        IQueryable<SecRole> RetrieveAllSecRoles();
        IQueryable<SecRole> RetrieveActiveSecRoles();
        IQueryable<SecRole> RetrieveInactiveSecRoles();
        SecRole RetrieveSecRole(int roleId);
        int CreateRole(SecRole secRole);
        int UpdateRole(SecRole secRole);
    }
}