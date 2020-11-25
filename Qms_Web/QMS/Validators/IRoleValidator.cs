using System.Collections.Generic;
using QmsCore.UIModel;

namespace QMS.Validators
{
	public interface IRoleValidator
	{
		(bool, string[]) UpdateRoleValuesAreValid(int roleId, string roleLabel, string[] permissionIds);
		(bool, string[]) CreateRoleValuesAreValid(string roleCode, string roleLabel, string[] permissionIds);
	}
}