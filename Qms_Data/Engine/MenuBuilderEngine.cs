using System;
using System.Collections.Generic;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QmsCore.Engine
{
    public class MenuBuilderEngine
    {
        User user;
        MenuBuilderRepository repository;
        List<Permission> userPermissions;

        List<ModuleMenuItem> menu;
        public MenuBuilderEngine(User u,MenuBuilderRepository r)
        {
            user = u;
            repository = r;
            userPermissions = new List<Permission>();
            setUserPermissions();
        }

        public bool isModuleLoaded(int moduleIdToCheck)
        {
            bool retval = false;
            foreach(ModuleMenuItem moduleMenuItem in menu)
            {
                if(moduleMenuItem.ModuleMenuItemId == moduleIdToCheck)
                {
                    retval=true;
                    break;
                }

            }
            return retval;
        }

        public List<ModuleMenuItem> BuildMenu()
        {
            menu = new List<ModuleMenuItem>();

            foreach(var userRole in user.UserRoles)
            {
                int roleId = userRole.RoleId;
                var moduleRoles = repository.RetrieveByRoleId(roleId);


                foreach(var moduleRole in moduleRoles)
                {
                    if(moduleRole.Module.DeletedAt == null && !isModuleLoaded(moduleRole.ModuleId))
                    {
                        Module module = new Module(moduleRole.Module);
                        ModuleMenuItem moduleMenuItem = module.ModuleMenuItem();
                        foreach(var item in moduleRole.Module.SysMenuitem)
                        {
                            int permissionId = item.PermissionId.Value;
                            if(userHasPermission(permissionId) && item.DeletedAt == null)
                            {
                                MenuItem menuItem= new MenuItem(item);
                                moduleMenuItem.MenuItems.Add(menuItem);
                            }
                            moduleMenuItem.MenuItems.Sort();
                        }
                        menu.Add(moduleMenuItem);
                    }
                }
            }
            menu.Sort();
            return menu;
        }

        private void setUserPermissions()
        {
            foreach(var userRole in user.UserRoles)
            {
                Role r = userRole.Role;
                foreach (Permission p in r.Permissions)
                {
                    if(p.IsActive)
                        userPermissions.Add(p);
                }
            }
        }

        private bool userHasPermission(int permissionId)
        {
            bool retval = false;
            foreach(Permission p in userPermissions)
            {
                if(p.PermissionId == permissionId)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }



    }//end class
}//end namespace