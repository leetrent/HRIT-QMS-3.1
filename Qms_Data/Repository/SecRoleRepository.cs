using System;
using System.Text;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class SecRoleRepository : ISecRoleRepository
    {
        private QMSContext _context;

        public SecRoleRepository()
        {
            _context = new QMSContext();
        }

        public SecRoleRepository(QMSContext context)
        {
            _context = context;
        }
        public IQueryable<SecRole> RetrieveAllSecRoles()
        {
            return _context.SecRole.Include(r => r.SecRolePermission).ThenInclude(r => r.Permission).OrderBy(r => r.RoleCode).AsNoTracking();
        }
        public IQueryable<SecRole> RetrieveActiveSecRoles()
        {
            return _context.SecRole.AsNoTracking().Where(p => p.DeletedAt == null).Include(r => r.SecRolePermission).ThenInclude(p => p.Permission).OrderBy(s => s.RoleCode);
        }
        public IQueryable<SecRole> RetrieveInactiveSecRoles()
        {
            return _context.SecRole.AsNoTracking().Where(p => p.DeletedAt != null).Include(r => r.SecRolePermission).ThenInclude(p => p.Permission).OrderBy(s => s.RoleCode);
        }
        public SecRole RetrieveSecRole(int roleId)
        {
            return _context.SecRole.AsNoTracking().Where(r => r.RoleId == roleId).Include(r => r.SecRolePermission).ThenInclude(p => p.Permission).SingleOrDefault();
        }
        public int CreateRole(SecRole secRole)
        {
            _context.SecRole.Add(secRole);
            _context.SaveChanges();
            return secRole.RoleId;
        }
        public int UpdateRole(SecRole secRole)
        {
            _context.SecRole.Update(secRole);
            return _context.SaveChanges();
        }
    }
}
