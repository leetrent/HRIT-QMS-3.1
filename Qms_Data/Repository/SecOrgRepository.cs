using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class SecOrgRepository : ISecOrgRepository
    {
        private QMSContext _context;

        public SecOrgRepository()
        {
            _context = new QMSContext();
        }
        public IQueryable<SecOrg> RetrieveAllSecOrganizations()
        {
            return _context.SecOrg.Where(s => s.OrgId > 1).OrderBy(s => s.OrgCode);
        }
        public IQueryable<SecOrg> RetrieveActiveSecOrganizations()
        {
            return _context.SecOrg.Where(p => p.DeletedAt == null && p.OrgId > 1).OrderBy(s => s.OrgCode);
        }
        public IQueryable<SecOrg> RetrieveInactiveSecOrganizations()
        {
            return _context.SecOrg.Where(p => p.DeletedAt != null & p.OrgId > 1).OrderBy(s => s.OrgCode);
        }
        public SecOrg RetrieveSecOrganization(int orgId)
        {
            return _context.SecOrg.Where(p => p.OrgId == orgId).SingleOrDefault();
        }
        public int CreateOrganization(SecOrg secOrg)
        {
            _context.SecOrg.Add(secOrg);
            _context.SaveChanges();
            return secOrg.OrgId;
        }
        public int UpdateOrganization(SecOrg secOrg)
        {
            _context.SecOrg.Update(secOrg);
            return _context.SaveChanges();
        }
    }
}
