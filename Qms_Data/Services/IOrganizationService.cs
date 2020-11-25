using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IOrganizationService
    {
        List<Organization> RetrieveAllOrganizations();
        List<Organization> RetrieveActiveOrganizations();
        List<Organization> RetrieveInactiveOrganizations();
        Organization RetrieveOrganization(int organizationId);
        int CreateOrganization(Organization newOrg, User user);
       
        int UpdateOrganization(int orgId, int? parentOrgId, string orgLabel, User user);

        int UpdateOrganization(int orgId, int parentOrgId, string orgLabel, User user);

        int DeactivateOrganization(int organizationId, User user);
        int ReactivateOrganization(int organizationId, User user);
    }
}