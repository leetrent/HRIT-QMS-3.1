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
    public class PermissionService : IPermissionService
    {
        private readonly ISecPermissionRepository _repository;
        private readonly ISecurityLogService _securityLogService;

        public PermissionService()
        {
            _repository = new SecPermissionRepository();
            _securityLogService = new SecurityLogService();
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE HELPER METHOD
        ////////////////////////////////////////////////////////////////////////////////////
        private List<Permission> IQueryableToList(IQueryable<SecPermission> secPermissions)
        {
            List<Permission> permissions = new List<Permission>();
            foreach (SecPermission secPermission in secPermissions)
            {
                permissions.Add(new Permission(secPermission));
            }
            return permissions;
        }
        public List<Permission> RetrieveAllPermissions()
        {
            return this.IQueryableToList(_repository.RetrieveAllSecPermissions());
        }
        public List<Permission> RetrieveActivePermissions()
        {
            return this.IQueryableToList(_repository.RetrieveActiveSecPermissions());
        }
        public List<Permission> RetrieveInactivePermissions()
        {
            return this.IQueryableToList(_repository.RetrieveInactiveSecPermissions());
        }
        public Permission RetrievePermission(int permissionId)
        {
            SecPermission secPermission = _repository.RetrieveSecPermission(permissionId);
            return new Permission(secPermission);
        }
  
        public int CreatePermission(string permissionCode, string permissionLabel, User user)
        {
            SecPermission secPermission = new SecPermission()
            {
                PermissionCode = permissionCode,
                PermissionLabel = permissionLabel,
                CreatedAt = DateTime.Now
            };
           
            int permissionId = _repository.CreatePermission(secPermission);
            this.LogTransaction(secPermission, SecurityLogTypeEnum.PERM_CREATED, user);
            return permissionId;
        }
        public int UpdatePermission(int permissionId, string perissionLabel, User user)
        {
            SecPermission secPermission = _repository.RetrieveSecPermission(permissionId);
     
            secPermission.PermissionLabel = perissionLabel;
            secPermission.UpdatedAt = DateTime.Now;

            this.LogTransaction(secPermission, SecurityLogTypeEnum.PERM_UPDATED, user);
            return _repository.UpdatePermission(secPermission);
        }

        public int DeactivatePermission(int permissionId, User user)
        {
            SecPermission secPermission = _repository.RetrieveSecPermission(permissionId);
            secPermission.DeletedAt = DateTime.Now;

            this.LogTransaction(secPermission, SecurityLogTypeEnum.PERM_DEACTIVATED, user);
            return _repository.UpdatePermission(secPermission);
        }

        public int ReactivatePermission(int permissionId, User user)
        {
            SecPermission secPermission = _repository.RetrieveSecPermission(permissionId);
            secPermission.DeletedAt = null;
            secPermission.UpdatedAt = DateTime.Now;

            this.LogTransaction(secPermission, SecurityLogTypeEnum.PERM_REACTIVATED, user);
            return _repository.UpdatePermission(secPermission);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE HELPER METHOD
        ////////////////////////////////////////////////////////////////////////////////////
        private void LogTransaction(SecPermission secPermission, string securityLogTypeEnum, User user)
        {
            Permission permission = new Permission
            {
                PermissionId = secPermission.PermissionId,
                PermissionCode = secPermission.PermissionCode,
                PermissionLabel = secPermission.PermissionLabel
            };
            _securityLogService.EntityUpdatedOrCreated(securityLogTypeEnum, permission, user);
        }
    }
}