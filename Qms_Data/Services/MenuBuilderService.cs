using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Engine;

namespace QmsCore.Services
{

    public class MenuBuilderService : IMenuBuilderService
    {
        MenuBuilderRepository repository;
        UserService userService;
        RoleService roleService;

        public MenuBuilderService()
        {
            repository = new MenuBuilderRepository();
            userService = new UserService();
            roleService = new RoleService();
        }

        public List<ModuleMenuItem> RetrieveMenuForUser()
        {

            var menu = new List<ModuleMenuItem>();
            var m1 = new ModuleMenuItem{Title = "Admin", ModuleMenuItemId = 1};
            m1.MenuItems.Add(new MenuItem{Controller = "User2",ControllerAction="Index", Title="User Admin",MenuItemId=1});
            m1.MenuItems.Add(new MenuItem{Controller = "RoleAdmin",ControllerAction="Index", Title="Role Admin",MenuItemId=2});
            m1.MenuItems.Add(new MenuItem{Controller = "PermissionAdmin",ControllerAction="Index", Title="Permission Admin",MenuItemId=3});
            menu.Add(m1);

            var m2= new ModuleMenuItem{Title = "Corrective Actions", ModuleMenuItemId=2};
            m2.MenuItems.Add(new MenuItem{Title="My Org's Actions", Controller = "CorrectiveAction",ControllerAction="Index",UseCase="VCAFO",MenuItemId=4});
            m2.MenuItems.Add(new MenuItem{Title="My Org's Archived Actions", Controller = "CorrectiveAction",ControllerAction="Index",UseCase="VACAFO",MenuItemId=5});
            m2.MenuItems.Add(new MenuItem{Title="Employees We Service", Controller = "CorrectiveAction",ControllerAction="Index",UseCase="VCAFP",MenuItemId=6});
            m2.MenuItems.Add(new MenuItem{Title="Employees We Service Archive", Controller = "CorrectiveAction",ControllerAction="Index",UseCase="VACAFP",MenuItemId=7});
            menu.Add(m2);

            return menu;    
        }

        public List<ModuleMenuItem> RetrieveMenuForUser(int userId)
        {
            User user = userService.RetrieveByUserId(userId);
            MenuBuilderEngine engine = new MenuBuilderEngine(user,repository);
            return engine.BuildMenu();
        }

    }
}