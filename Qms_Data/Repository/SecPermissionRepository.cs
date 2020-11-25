using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class SecPermissionRepository : ISecPermissionRepository
    {
        private QMSContext _context;

        public SecPermissionRepository()
        {
            _context = new QMSContext();
        }
        public IQueryable<SecPermission> RetrieveAllSecPermissions()
        {
            return _context.SecPermission.AsNoTracking().OrderBy(s => s.PermissionId);
        }
        public IQueryable<SecPermission> RetrieveActiveSecPermissions()
        {
            return _context.SecPermission.Where(p => p.DeletedAt == null).OrderBy(s => s.PermissionLabel).AsNoTracking();
        }
        public IQueryable<SecPermission> RetrieveInactiveSecPermissions()
        {
            return _context.SecPermission.AsNoTracking().Where(p => p.DeletedAt != null).OrderBy(s => s.PermissionId);
        }
        public SecPermission RetrieveSecPermission(int permissionId)
        {
            return _context.SecPermission.AsNoTracking().Where(p => p.PermissionId == permissionId).SingleOrDefault();
        }
        public int CreatePermission(SecPermission secPermission)
        {
            _context.SecPermission.Add(secPermission);
            _context.SaveChanges();
            return secPermission.PermissionId;
        }
        public int UpdatePermission(SecPermission secPermission)
        {
            _context.SecPermission.Update(secPermission);
            return _context.SaveChanges();
        }
     }
}
