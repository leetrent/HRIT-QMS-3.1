using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;
using QMS.Extensions;
using QMS.Constants;
using QMS.ViewModels;

namespace QMS.ViewComponents
{
    public class MenuNavViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if (HttpContext != null
                   && HttpContext.Session != null
                   && HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) != null
                   && HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY) != null)
            {
                List<ModuleMenuItem> moduleMenuItems = HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);
                return View(moduleMenuItems);
            }

            return View(new List<ModuleMenuItem>());
        }
    }
}