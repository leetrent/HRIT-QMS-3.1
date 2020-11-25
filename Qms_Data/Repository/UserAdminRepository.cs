using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class UserAdminRepository : IUserAdminRepository
    {
        QMSContext context;
        public UserAdminRepository()
        {
            context = new QMSContext();
        }

        public IQueryable<SecPermission> RetrieveSecPermissions()
        {
            return context.SecPermission.OrderBy(s => s.PermissionId);
        }
        public IList<SecRolePermission> RetrieveSecRolePermissions()
        {
            return context.SecRolePermission.OrderBy(s => s.RoleId).ThenBy(s => s.PermissionId).ToList();
        }


        //public IQueryable<SecUserOrgRole> RetrieveSecUserOrgRoles()
        //{
        //    return context.SecUserOrgRole.Where(r => r.RoleId == 1);
        //}

        public SecRole RetrieveSecRole(int roleId)
        {
            return context.SecRole.AsNoTracking().Where(r => r.RoleId == roleId).Include(r => r.SecRolePermission).SingleOrDefault();
        }

        public IQueryable<SecRole> RetrieveSecRoles()
        {
            return context.SecRole.Include(r => r.SecRolePermission).ThenInclude(p => p.Permission); //.OrderBy(p => p.RoleId).ThenBy(p => p.SecRolePermission);
        }

        //public int SavePermission(string newPermissionCode, string newPermissionLabel) //(SecPermission secPermission)
        public int SavePermission(SecPermission secPermission)
        {
            context.SecPermission.Add(secPermission);
            context.SaveChanges();
            return secPermission.PermissionId;
        }

        public int CreatePermission(SecPermission secPermission)
        {
            context.SecPermission.Add(secPermission);
            context.SaveChanges();
            return secPermission.PermissionId;
        }
        public SecPermission RetrieveSecPermission(int permissionId)
        {
            return context.SecPermission.Where(p => p.PermissionId == permissionId).SingleOrDefault();
        }

        public int UpdatePermission(SecPermission secPermission)
        {
            context.SecPermission.Update(secPermission);
            return context.SaveChanges();
        }
        public int DeletePermission(SecPermission secPermission)
        {
            context.SecPermission.Update(secPermission);
            context.SaveChanges();
            return secPermission.PermissionId;
        }
        public int CreateRole(SecRole secRole)
        {
            context.SecRole.Add(secRole);
            context.SaveChanges();
            return secRole.RoleId;
        }
        public int UpdateRole(SecRole secRole)
        {
            context.SecRole.Update(secRole);
            return context.SaveChanges();
        }
        public int CreateSecRolePermission(SecRolePermission secRolePermission)
        {
            context.SecRolePermission.Add(secRolePermission);
            context.SaveChanges();
            return secRolePermission.RolePermissionId;

        }

        public IList<SecUserRole> RetrieveSecUserRoles()
        {
            return context.SecUserRole.OrderBy(s => s.UserOrgRoleId).ThenBy(s => s.UserId).ThenBy(s => s.RoleId).ToList();
        }

        public SecUser RetrieveSecUser(int userId)
        {
            return context.SecUser.Where(u => u.UserId == userId).SingleOrDefault();
        }

        public int CreateUser(SecUser secUser)
        {
            context.SecUser.Add(secUser);
            context.SaveChanges();
            return secUser.UserId;
        }

        public int UpdateUser(SecUser secUser)
        {
            context.SecUser.Update(secUser);
            return context.SaveChanges();
        }

        public int CreateOrg(SecOrg secOrg)
        {
            context.SecOrg.Add(secOrg);
            context.SaveChanges();
            return secOrg.OrgId;
        }
    }

}
