using System.Collections.Generic;
using QmsCore.Services;
using QmsCore.UIModel;
using QmsCore.QmsException;

namespace QMS.Validators
{
	public class UserValidator
	{
		private readonly IUserService _userService;
		private readonly IOrganizationService _organizationService;

		public UserValidator(IUserService usrSvc, IOrganizationService orgSvc)
		{
			_userService = usrSvc;
			_organizationService = orgSvc;
		}

		public (bool, string[]) CreateUserValuesAreValid(int orgId, int managerId, string emailAddress, string displayName, string[] selectedRoleIdsForUser)
        {
			List<string> errMsgs = new List<string>();


			// IS EMAIL ADDRESS VALID?
			if (EmailValidator.IsValidEmail(emailAddress) == false)
            {
				errMsgs.Add($"Invalid email address: \"{emailAddress}\"");
			}
			else
            {
				// DOES USER ALREADY EXIST?
				if (this.UserAlreadyExists(emailAddress))
				{
					errMsgs.Add($"User already exists with this email address: \"{emailAddress}\"");
				}
			}

			// DOES ORGANIZATION EXIST?
			if ( this.OrganizationExits(orgId) == false)
            {
				errMsgs.Add($"An organizaation with an org_id of  \"{orgId}\" does not exist in the QMS system.");
			}
			else
            {
				// IS SELECTED MANAGER A MANAGER IN SELECTED ORGANIZATION
				if ( managerId > 0)
                {
					if ( this.IsManagerInOrganization(orgId, managerId) == false)
                    {
						errMsgs.Add($"User with a User ID of \"{managerId}\" is not a a manager in organization with an Organization ID of  \"{orgId}\".");
					}

				}
            }

			if (errMsgs.Count > 0)
			{
				errMsgs.Add("Create user failed due to the following validation error(s).");
				return (false, errMsgs.ToArray());
			}
			return (true, new string[0]);
		}

		public bool UserAlreadyExists(string userEmailAddress)
        {
			try
            {
				User dbUser = _userService.RetrieveByEmailAddress(userEmailAddress);
				return (dbUser != null);
			}
			catch (UserNotFoundException unfe)
            {
				System.Console.WriteLine(unfe.Message);
				return false;
            }
		}

		public bool OrganizationExits(int orgId)
        {
			Organization org = _organizationService.RetrieveOrganization(orgId);
			return (org != null && org.OrgId == orgId);
		}

		public bool IsManagerInOrganization(int orgId, int managerId)
        {
			List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(orgId);
			foreach ( User userInOrg in usersInOrg)
            {
				if ( userInOrg.UserId == managerId)
                {
					return true;
				}
			}
			return false;
		}

	}
}