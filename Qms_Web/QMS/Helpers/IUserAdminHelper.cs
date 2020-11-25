using System.Collections.Generic;
using QmsCore.UIModel;

namespace QMS.Helpers
{
	public interface IUserAdminHelper
	{
		Role CreateNewRole(string roleCode, string roleLabel, string[] selectedPermissionIdStrings, string roleId = null);
		void ProcessPermissionCheckboxesForAllRoles(List<Role> allRoles, List<Permission> allPermissions);
	}
}