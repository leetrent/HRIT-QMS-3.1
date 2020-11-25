using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IRoleService
    {
        List<Role> RetrieveAllRoles();
        List<Role> RetrieveActiveRoles();
        List<Role> RetrieveInactiveRoles();
        Role RetrieveRole(int roleId);
        int CreateRole(Role role);
        int UpdateRole(Role uiRole);
        int DeactivateRole(int roleId);
        int ReactivateRole(int roleId);
    }
}