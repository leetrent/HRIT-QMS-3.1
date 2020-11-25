using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QMS.Validators
{
	public class RoleValidator : IRoleValidator
	{
		private readonly IRoleService _roleService;

		public RoleValidator(IRoleService roleSvc)
		{
			_roleService = roleSvc;
		}

		public (bool, string[]) UpdateRoleValuesAreValid(int roleId, string roleLabel, string[] permissionIds)
		{
			List<string> errMsgs = new List<string>();

			if (String.IsNullOrEmpty(roleLabel) == true
					|| String.IsNullOrWhiteSpace(roleLabel) == true)
			{
				errMsgs.Add("Role label is requried.");
			}

			if (roleLabel != null && roleLabel.Length > 100)
			{
				errMsgs.Add("Maximum length for role label is 100 characters.");
			}
			if (permissionIds == null || permissionIds.Length < 1)
			{
				errMsgs.Add("At least one permission is requred.");
			}
			if (String.IsNullOrEmpty(roleLabel) == false 
					&& String.IsNullOrWhiteSpace(roleLabel) == false)
			{
				string testRoleLabel = roleLabel.Trim().ToUpper();
				List<Role> allRoles = _roleService.RetrieveAllRoles();

				foreach (Role role in allRoles)
				{
					if (role.RoleId != roleId 
							&& role.RoleLabel.Trim().ToUpper().Equals(testRoleLabel))
					{
						errMsgs.Add($"Role label '{roleLabel}' is already in use");
					}
				}
			}
			if (errMsgs.Count > 0)
			{
				errMsgs.Add("Update role failed due to the above validation error(s).");
				return (false, errMsgs.ToArray());
			}
			return (true, new string[0]);
		}

		public (bool, string[]) CreateRoleValuesAreValid(string roleCode, string roleLabel, string[] permissionIds)
		{
			List<string> errMsgs = new List<string>();

			if (String.IsNullOrEmpty(roleCode) == true
					|| string.IsNullOrWhiteSpace(roleCode) == true)
			{
				errMsgs.Add("ROLE_CODE is requred.");
			}
			if (String.IsNullOrEmpty(roleLabel) == true
					|| string.IsNullOrWhiteSpace(roleLabel) == true)
			{
				errMsgs.Add("Role label is requred.");
			}
			if (roleCode != null && roleCode.Length > 100)
			{
				errMsgs.Add("Maximum length for ROLE_CODE is 100 characters.");
			}
			if (roleLabel != null && roleLabel.Length > 100)
			{
				errMsgs.Add("Maximum length for role label is 100 characters.");
			}
			if (permissionIds == null || permissionIds.Length < 1)
            {
				errMsgs.Add("At least one permission is requred.");
			}

			string testRoleCode  = (roleCode == null)	? "" : roleCode.Trim().ToUpper();
			string testRoleLabel = (roleLabel == null)	? "" : roleLabel.Trim().ToUpper();

			List<Role> allRoles = _roleService.RetrieveAllRoles();
			foreach (Role role in allRoles)
			{
				if (role.RoleCode.Trim().ToUpper().Equals(testRoleCode))
				{
					errMsgs.Add($"ROLE_CODE '{role.RoleCode}' is already in use");
				}
				if (role.RoleLabel.Trim().ToUpper().Equals(testRoleLabel))
				{
					errMsgs.Add($"Role label '{role.RoleLabel}' is already in use");
				}
			}

			if (errMsgs.Count > 0)
			{
				errMsgs.Add("Create role failed due to the above validation error(s).");
				return (false, errMsgs.ToArray());
			}
			return (true, new string[0]);
		}
	}
}