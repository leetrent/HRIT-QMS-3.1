//using System;
//using System.Collections.Generic;
//using QmsCore.UIModel;
//using QMS.ViewModels;

//namespace QMS.Utils
//{
//    public static class DELETEMenuUtil
//    {
//        private static List<MenuItemViewModel> GetAllMenuItems()
//        {
//            return new List<MenuItemViewModel>
//            {
//                new MenuItemViewModel
//                { 
//                    MenuItemId = 1,
//                    CategoryId = 1, 
//                    MenuLabel = "Create Corrective Action", 
//                    PermissionCode = "CREATE_CORRECTIVE_ACTION", 
//                    ControllerName = "CorrectiveActions", 
//                    ActionName = "Create"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 2,
//                    CategoryId = 2,
//                    MenuLabel = "My Corrective Actions",  //"My Corrective Actions"
//                    PermissionCode = "VIEW_CORRECTIVE_ACTIONS_FOR_USER",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VCAFU"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 3,
//                    CategoryId = 2,
//                    MenuLabel = "My Org's Actions", //"My Center's Actions"
//                    PermissionCode = "VIEW_CORRECTIVE_ACTIONS_FOR_ORG",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VCAFO"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 4,
//                    CategoryId = 9,
//                    MenuLabel = "Employees We Service", // "Actions by Employee POID"
//                    PermissionCode = "VIEW_CORRECTIVE_ACTIONS_FOR_POID",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VCAFP"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 5,
//                    CategoryId = 3,
//                    MenuLabel = "All Corrective Actions", // "All Corrective Actions"
//                    PermissionCode = "VIEW_ALL_CORRECTIVE_ACTIONS",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VCAALL"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 6,
//                    CategoryId = 4,
//                    MenuLabel = "My Archived Actions",  //"My Archived Actions"
//                    PermissionCode = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_USER",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VACAFU"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 7,
//                    CategoryId = 4,
//                    MenuLabel = "My Org's Archived Actions", //"My Center's Archived Actions"
//                    PermissionCode = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_ORG",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VACAFO"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 8,
//                    CategoryId = 10,
//                    MenuLabel = "Employees We Service Archive",  // "Archived Actions by Employee POID"
//                    PermissionCode = "VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_POID",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VACAFP"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 9,
//                    CategoryId = 5,
//                    MenuLabel = "All Archived Corrective Actions", // "All Archived Corrective Actions"
//                    PermissionCode = "VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS",
//                    ControllerName = "CorrectiveActions",
//                    ActionName = "Index",
//                    UseCase = "VACAALL"
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 10,
//                    CategoryId = 6,
//                    MenuLabel = "User Admin",
//                    PermissionCode = "CREATE_USER",
//                    ControllerName = "User2",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 11,
//                    CategoryId = 6,
//                    MenuLabel = "User Admin",
//                    PermissionCode = "RETRIEVE_USER",
//                    ControllerName = "User2",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 12,
//                    CategoryId = 6,
//                    MenuLabel = "User Admin",
//                    PermissionCode = "UPDATE_USER",
//                    ControllerName = "User2",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 13,
//                    CategoryId = 6,
//                    MenuLabel = "User Admin",
//                    PermissionCode = "DEACTIVATE_USER",
//                    ControllerName = "User2",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 14,
//                    CategoryId = 6,
//                    MenuLabel = "User Admin",
//                    PermissionCode = "REACTIVATE_USER",
//                    ControllerName = "User2",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 15,
//                    CategoryId = 7,
//                    MenuLabel = "Role Admin",
//                    PermissionCode = "CREATE_ROLE",
//                    ControllerName = "Role",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 16,
//                    CategoryId = 7,
//                    MenuLabel = "Role Admin",
//                    PermissionCode = "RETRIEVE_ROLE",
//                    ControllerName = "Role",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 17,
//                    CategoryId = 7,
//                    MenuLabel = "Role Admin",
//                    PermissionCode = "UPDATE_ROLE",
//                    ControllerName = "Role",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 18,
//                    CategoryId = 7,
//                    MenuLabel = "Role Admin",
//                    PermissionCode = "DEACTIVATE_ROLE",
//                    ControllerName = "Role",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 19,
//                    CategoryId = 7,
//                    MenuLabel = "Role Admin",
//                    PermissionCode = "REACTIVATE_ROLE",
//                    ControllerName = "Role",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },

//                new MenuItemViewModel
//                {
//                    MenuItemId = 20,
//                    CategoryId = 8,
//                    MenuLabel = "Permission Admin",
//                    PermissionCode = "CREATE_PERMISSION",
//                    ControllerName = "Permission",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 21,
//                    CategoryId = 8,
//                    MenuLabel = "Permission Admin",
//                    PermissionCode = "RETRIEVE_PERMISSION",
//                    ControllerName = "Permission",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 22,
//                    CategoryId = 8,
//                    MenuLabel = "Permission Admin",
//                    PermissionCode = "UPDATE_PERMISSION",
//                    ControllerName = "Permission",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 23,
//                    CategoryId = 8,
//                    MenuLabel = "Permission Admin",
//                    PermissionCode = "DEACTIVATE_PERMISSION",
//                    ControllerName = "Permission",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },
//                new MenuItemViewModel
//                {
//                    MenuItemId = 24,
//                    CategoryId = 8,
//                    MenuLabel = "Permission Admin",
//                    PermissionCode = "REACTIVATE_PERMISSION",
//                    ControllerName = "Permission",
//                    ActionName = "Index",
//                    UseCase = string.Empty
//                },

//            };
//        }

//        private static List<string> GetPermissionCodesForUser(User user)
//        {
//            List<string> permissionCodes = new List<string>();      

//            foreach (UserRole userRole in user.UserRoles)
//            {
//                Role role = userRole.Role;
//                foreach (Permission permission in role.Permissions)
//                {
//                    permissionCodes.Add(permission.PermissionCode);
//                }
//            }
            
//            return permissionCodes;
//        }

//        /*
//        public static List<MenuItemViewModel> GetMenuItemsforUser(User user)
//        {
//            List<string> permissionCodes                = MenuUtil.GetPermissionCodesForUser(user);
//            List<MenuItemViewModel> allMenuItems        = MenuUtil.GetAllMenuItems();
//            List<MenuItemViewModel> menuItemsForUser    = new List<MenuItemViewModel>();

//            foreach (string permissionCode in permissionCodes)
//            {
//                foreach (MenuItemViewModel menuItem in allMenuItems)
//                {
//                    if (menuItem.PermissionCode.Equals(permissionCode))
//                    {
//                        menuItemsForUser.Add(menuItem);
//                    }
//                }
//            }
//            return menuItemsForUser;
//        }
//        */

//        public static HashSet<MenuItemViewModel> GetMenuItemsforUser(User user)
//        {
//            string logSnippet ="[MenuUtil][GetMenuItemsforUser] => ";

//            List<string> permissionCodesList                = MenuUtil.GetPermissionCodesForUser(user);
//            List<MenuItemViewModel> allMenuItemsList        = MenuUtil.GetAllMenuItems();
//            HashSet<MenuItemViewModel> menuItemsForUserSet  = new HashSet<MenuItemViewModel>();

//            foreach (string permissionCode in permissionCodesList)
//            {
//                foreach (MenuItemViewModel menuItem in allMenuItemsList)
//                {
//                    if (menuItem.PermissionCode.Equals(permissionCode))
//                    {
//                        menuItemsForUserSet.Add(menuItem);
//                    }
//                }
//            }

//            Console.WriteLine(logSnippet + $"(permissionCodesList.Count): {permissionCodesList.Count}");
//            Console.WriteLine(logSnippet + $"(allMenuItemsList.Count)...: {allMenuItemsList.Count}");
//            Console.WriteLine(logSnippet +$"(menuItemsForUserSet.Count).: {menuItemsForUserSet.Count}");

//            return menuItemsForUserSet;
//        }
           
//    }
//}