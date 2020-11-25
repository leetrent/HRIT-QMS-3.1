using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QMS.Extensions;
using QMS.ViewModels;
using QMS.Constants;

namespace QMS.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                List<MenuItemViewModel> menuBuilder = HttpContext.Session.GetObject<List<MenuItemViewModel>>(MiscConstants.MENU_SESSION_KEY);
                return View(menuBuilder);
            }

            return View();
        }
    }
}