using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;

namespace QMS.ViewComponents
{
    public class UAPermissionsActiveViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;

        public UAPermissionsActiveViewComponent(IPermissionService permSvc)
        {
            _permissionService = permSvc;
        }

        public IViewComponentResult Invoke()
        {
            return View(_permissionService.RetrieveActivePermissions());
        }
    }
}