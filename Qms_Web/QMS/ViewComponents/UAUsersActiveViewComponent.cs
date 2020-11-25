using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;

namespace QMS.ViewComponents
{
    public class UAUsersActiveViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UAUsersActiveViewComponent(IUserService usrSvc)
        {
            _userService = usrSvc;
        }

        public IViewComponentResult Invoke()
        {
            return View(_userService.RetrieveActiveUsers());
        }
    }
}