using System.Collections.Generic;
using QmsCore.UIModel;

namespace QMS.Validators
{
	public interface IPermissionValidator
	{
		(bool, string) UpdatePermissionValuesAreValid(int permissionId, string permissionLabel);
		(bool, string[]) CreatePermissionValuesAreValid(string permissionCode, string permissionLabel);
	}
}