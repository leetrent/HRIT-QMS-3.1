using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;
using QmsCore.Repository;
using QmsCore.QmsException;
using System.Linq;

namespace QmsCore.Services
{
    public class OrganizationService : IOrganizationService
    {
        private ISecOrgRepository _repository;
        private readonly ISecurityLogService _securityLogService;

        public OrganizationService()
        {
            _repository = new SecOrgRepository();
            _securityLogService = new SecurityLogService();
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE HELPER METHOD
        ////////////////////////////////////////////////////////////////////////////////////
        private List<Organization> IQueryableToList(IQueryable<SecOrg> secOrganizations)
        {
            List<Organization> organizations = new List<Organization>();
            foreach (SecOrg secOrganization in secOrganizations)
            {
                organizations.Add(new Organization(secOrganization));
            }
            return organizations;
        }
        public List<Organization> RetrieveAllOrganizations()
        {
            return this.IQueryableToList(_repository.RetrieveAllSecOrganizations());
        }
        public List<Organization> RetrieveActiveOrganizations()
        {
            return this.IQueryableToList(_repository.RetrieveActiveSecOrganizations());
        }
        public List<Organization> RetrieveInactiveOrganizations()
        {
            return this.IQueryableToList(_repository.RetrieveInactiveSecOrganizations());
        }
        public Organization RetrieveOrganization(int organizationId)
        {
            SecOrg secOrganization = _repository.RetrieveSecOrganization(organizationId);
            return new Organization(secOrganization);
        }

        public int CreateOrganization(Organization newOrg, User user)
        {
            SecOrg secOrganization = new SecOrg()
            {
                OrgCode = newOrg.OrgCode,
                OrgLabel = newOrg.OrgLabel,
                ParentOrgId = newOrg.ParentOrgId,
                CreatedAt = DateTime.Now
            };
         
            int orgId = _repository.CreateOrganization(secOrganization);
            this.LogTransaction(secOrganization, SecurityLogTypeEnum.ORG_CREATED, user);
            return orgId;
        }
        
        public int UpdateOrganization(int orgId, int parentOrgId, string orgLabel, User user)
        {
            SecOrg secOrganization = _repository.RetrieveSecOrganization(orgId);

            secOrganization.ParentOrgId = parentOrgId;
            secOrganization.OrgLabel = orgLabel;
            secOrganization.UpdatedAt = DateTime.Now;

            this.LogTransaction(secOrganization, SecurityLogTypeEnum.ORG_UPDATED, user);
            return _repository.UpdateOrganization(secOrganization);
        }

        public int UpdateOrganization(int orgId, int? parentOrgId, string orgLabel, User user)
        {
            SecOrg secOrganization = _repository.RetrieveSecOrganization(orgId);

            secOrganization.ParentOrgId = parentOrgId;
            secOrganization.OrgLabel = orgLabel;
            secOrganization.UpdatedAt = DateTime.Now;

            this.LogTransaction(secOrganization, SecurityLogTypeEnum.ORG_UPDATED, user);
            return _repository.UpdateOrganization(secOrganization);
        }

        public int DeactivateOrganization(int organizationId, User user)
        {
            SecOrg secOrganization = _repository.RetrieveSecOrganization(organizationId);
            secOrganization.DeletedAt = DateTime.Now;
 
            this.LogTransaction(secOrganization, SecurityLogTypeEnum.ORG_DEACTIVATED, user);
            return _repository.UpdateOrganization(secOrganization);
        }

        public int ReactivateOrganization(int organizationId, User user)
        {
            SecOrg secOrganization = _repository.RetrieveSecOrganization(organizationId);
            secOrganization.DeletedAt = null;
            secOrganization.UpdatedAt = DateTime.Now;

            this.LogTransaction(secOrganization, SecurityLogTypeEnum.ORG_REACTIVATED, user);
            return _repository.UpdateOrganization(secOrganization);
        }
        
        private void LogTransaction(SecOrg secOrg, string securityLogTypeEnum, User user)
        {
            Organization organization = new Organization 
            {
                OrgId = secOrg.OrgId,
                OrgCode = secOrg.OrgCode,
                OrgLabel = secOrg.OrgLabel
            };
            _securityLogService.EntityUpdatedOrCreated(securityLogTypeEnum, organization, user);
        }
    }
}