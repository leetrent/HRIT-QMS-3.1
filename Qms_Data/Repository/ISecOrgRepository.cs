using System;
using QmsCore.Model;
using System.Linq;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public interface ISecOrgRepository
    {
        IQueryable<SecOrg> RetrieveAllSecOrganizations();
        IQueryable<SecOrg> RetrieveActiveSecOrganizations();
        IQueryable<SecOrg> RetrieveInactiveSecOrganizations();
        SecOrg RetrieveSecOrganization(int orgId);
        int CreateOrganization(SecOrg secOrg);
        int UpdateOrganization(SecOrg secOrg);
    }
}