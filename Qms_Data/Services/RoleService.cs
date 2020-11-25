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
    public class RoleService : IRoleService
    {
        private ISecRoleRepository _repository;

        public RoleService()
        {
            _repository = new SecRoleRepository();
        }

        public RoleService(QMSContext context)
        {
            _repository = new SecRoleRepository(context);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE HELPER METHOD
        ////////////////////////////////////////////////////////////////////////////////////
        private List<Role> IQueryableToList(IQueryable<SecRole> secRoles)
        {
            List<Role> roles = new List<Role>();
            foreach (SecRole secRole in secRoles)
            {
                roles.Add(new Role(secRole));
            }
            return roles;
        }
        public List<Role> RetrieveAllRoles()
        {
            return this.IQueryableToList(_repository.RetrieveAllSecRoles());
        }
        public List<Role> RetrieveActiveRoles()
        {
            return this.IQueryableToList(_repository.RetrieveActiveSecRoles());
        }
        public List<Role> RetrieveInactiveRoles()
        {
            return this.IQueryableToList(_repository.RetrieveInactiveSecRoles());
        }
        public Role RetrieveRole(int roleId)
        {
            SecRole secRole = _repository.RetrieveSecRole(roleId);
            return new Role(secRole);
        }

        public int CreateRole(Role role)
        {
            SecRole secRole = new SecRole()
            {
                RoleCode = role.RoleCode,
                RoleLabel = role.RoleLabel,
                CreatedAt = DateTime.Now
            };
            foreach (Permission permission in role.Permissions)
            {
                SecRolePermission secRolePermission = new SecRolePermission();
                secRolePermission.PermissionId = permission.PermissionId;
                secRole.SecRolePermission.Add(secRolePermission);
            }
            return _repository.CreateRole(secRole);
        }

        public int UpdateRole(Role uiRole)
        {
            SecRole oldRole = _repository.RetrieveSecRole(uiRole.RoleId);

            SecRole updatedRole = new SecRole
            {
                RoleId = oldRole.RoleId,
                RoleCode = oldRole.RoleCode,
                RoleLabel = uiRole.RoleLabel,
                UpdatedAt = DateTime.Now
            };

            HashSet<int> oldPermissionIdSet = new HashSet<int>(oldRole.SecRolePermission.Select(p => p.PermissionId));
            HashSet<int> newPermissionIdSet = new HashSet<int>(uiRole.Permissions.Select(p => p.PermissionId));

            HashSet<int> redundantPermissionIdSet   = new HashSet<int>();
            HashSet<int> softDeletePermissionIdSet  = new HashSet<int>();
            HashSet<int> addToPermissionIdSet       = new HashSet<int>();

            foreach (int oldPermissionId in oldPermissionIdSet)
            {
                if (newPermissionIdSet.Contains(oldPermissionId))
                {
                    // If the old permission is in the new permission set,
                    // mark it as redundant because this sec_role_permission
                    // row is already in the database.
                    redundantPermissionIdSet.Add(oldPermissionId);
                }
                else
                {
                    // If the old permission set is not in the new permission set,
                    // mark it for soft deletion
                    softDeletePermissionIdSet.Add(oldPermissionId);
                }
            }

            foreach (int newPermissionId in newPermissionIdSet)
            {
                // If the new permission is not in the old permission set,
                // mark it for addition.
                if (oldPermissionIdSet.Contains(newPermissionId) == false)
                {
                    addToPermissionIdSet.Add(newPermissionId);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            // Redundant or Reactivated Permissions
            ///////////////////////////////////////////////////////////////////////////////
            foreach (int redundantPermissionId in redundantPermissionIdSet)
            {
                foreach (SecRolePermission oldSecRolePerm in oldRole.SecRolePermission)
                {
                    if (oldSecRolePerm.PermissionId == redundantPermissionId)
                    {
                        if (oldSecRolePerm.DeletedAt == null)
                        {
                            //  If DeletedAt IS NULL, do not add this SecRolePermission to the newly
                            //  created role because there haven't been any changes.
                        }
                        else
                        {
                            // If DeletedAt IS NOT NULL, this SecRolePermission needs to be 
                            // reactivated by setting DeletedAt to NULL and UpdatedAt to DateTime.Now
                            SecRolePermission reactivatedSecRolePerm = new SecRolePermission
                            {
                                RolePermissionId = oldSecRolePerm.RolePermissionId,
                                RoleId = oldSecRolePerm.RoleId,
                                PermissionId = oldSecRolePerm.PermissionId,
                                CreatedAt = oldSecRolePerm.CreatedAt,
                                DeletedAt = null,
                                UpdatedAt = DateTime.Now
                            };
                            
                            updatedRole.SecRolePermission.Add(reactivatedSecRolePerm);
                        }
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            // SOFT DELETES
            // -- SecRolePermission in old SecRole but not in new UIModel.Row
            // -- set DeletedAt to DateTime.Now
            ///////////////////////////////////////////////////////////////////////////////
            foreach (int softDeletePermissionId in softDeletePermissionIdSet)
            {
                foreach (SecRolePermission oldSecRolePerm in oldRole.SecRolePermission)
                {
                    if (oldSecRolePerm.PermissionId == softDeletePermissionId)
                    {
                        SecRolePermission reactivatedSecRolePerm = new SecRolePermission
                        {
                            RolePermissionId = oldSecRolePerm.RolePermissionId,
                            RoleId = oldSecRolePerm.RoleId,
                            PermissionId = oldSecRolePerm.PermissionId,
                            CreatedAt = oldSecRolePerm.CreatedAt,
                            DeletedAt = DateTime.Now,
                            UpdatedAt = oldSecRolePerm.UpdatedAt
                        };
                        updatedRole.SecRolePermission.Add(reactivatedSecRolePerm);
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            // NEW PERMISSIONS
            // UIModel.RolePermission in new UIModel.Role but not in old SecRole
            // Add to SecRole's SecRolePermission collection before saving new version
            // of SecRole to database.
            ///////////////////////////////////////////////////////////////////////////////
            foreach (int addToPermissionId in addToPermissionIdSet)
            {
                SecRolePermission newSecRolePerm = new SecRolePermission
                {
                    RoleId = updatedRole.RoleId,
                    PermissionId = addToPermissionId
                };

                updatedRole.SecRolePermission.Add(newSecRolePerm);
            }

            return _repository.UpdateRole(updatedRole);
        }


        public int DeactivateRole(int roleId)
        {
            SecRole secRole = _repository.RetrieveSecRole(roleId);
            secRole.DeletedAt = DateTime.Now;
            secRole.SecRolePermission.Clear();
            return _repository.UpdateRole(secRole);
        }

        public int ReactivateRole(int roleId)
        {
            SecRole secRole = _repository.RetrieveSecRole(roleId);
            secRole.DeletedAt = null;
            secRole.UpdatedAt = DateTime.Now;
            secRole.SecRolePermission.Clear();

            return _repository.UpdateRole(secRole);
        }
    }
}