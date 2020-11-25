using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;

namespace QMS.ViewComponents
{
    public class UAPermissionsInactiveViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;

        public UAPermissionsInactiveViewComponent(IPermissionService permSvc)
        {
            _permissionService = permSvc;
        }

        public IViewComponentResult Invoke()
        {
            return View(_permissionService.RetrieveInactivePermissions());
        }
    }
}