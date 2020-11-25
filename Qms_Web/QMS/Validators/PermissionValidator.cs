using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QMS.Validators
{
	public class PermissionValidator : IPermissionValidator
	{
		private readonly IPermissionService _permissionService;

		public PermissionValidator(IPermissionService permSvc)
		{
			_permissionService = permSvc;
		}

		public (bool, string) UpdatePermissionValuesAreValid(int permissionId, string permissionLabel)
		{
			if (String.IsNullOrEmpty(permissionLabel) == true
					|| string.IsNullOrWhiteSpace(permissionLabel) == true)
			{
				return (false, "Permission label is requried.");
			}

			if (permissionLabel != null && permissionLabel.Length > 100)
			{
				return (false, "Maximum length for permission label is 100 characters.");
			}

			string testPermissionLabel = permissionLabel.Trim().ToUpper();
			List<Permission> allPermissions = _permissionService.RetrieveAllPermissions();

			foreach (Permission permission in allPermissions)
			{
				if (permission.PermissionId != permissionId 
						&& permission.PermissionLabel.Trim().ToUpper().Equals(testPermissionLabel))
				{
					return (false, $"Permission label '{permissionLabel}' is already in use");
				}
			}

			return (true, null);
		}

		public (bool, string[]) CreatePermissionValuesAreValid(string permissionCode, string permissionLabel)
		{
			List<string> errMsgs = new List<string>();

			if (String.IsNullOrEmpty(permissionCode) == true
					|| string.IsNullOrWhiteSpace(permissionCode) == true)
			{
				errMsgs.Add("PERMISSION_CODE is requred.");
			}
			if (String.IsNullOrEmpty(permissionLabel) == true
					|| string.IsNullOrWhiteSpace(permissionLabel) == true)
			{
				errMsgs.Add("Permission label is requred.");
			}
			if (permissionCode != null && permissionCode.Length > 100)
			{
				errMsgs.Add("Maximum length for PERMISSION_CODE is 100 characters.");
			}
			if (permissionLabel != null && permissionLabel.Length > 100)
			{
				errMsgs.Add("Maximum length for permission label is 100 characters.");
			}

			string testPermissionCode	= permissionCode.Trim().ToUpper();
			string testPermissionLabel	= permissionLabel.Trim().ToUpper();

			List<Permission> allPermissions = _permissionService.RetrieveAllPermissions();
			foreach (Permission permission in allPermissions)
			{
				if (permission.PermissionCode.Trim().ToUpper().Equals(testPermissionCode))
				{
					errMsgs.Add($"PERMISSION_CODE '{permission.PermissionCode}' is already in use");
				}
				if (permission.PermissionLabel.Trim().ToUpper().Equals(testPermissionLabel))
				{
					errMsgs.Add($"Permission label '{permission.PermissionLabel}' is already in use");
				}
			}

			if (errMsgs.Count > 0)
            {
				return (false, errMsgs.ToArray());
            }
			return (true, new string[0]);
		}
	}
}