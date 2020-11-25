using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.UIModel;
using QmsCore.Services;
using QmsCore.Model;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Extensions;

namespace QMS.ViewComponents
{
    public class CAAssigneeViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public CAAssigneeViewComponent(IUserService usrSvc)
        {
            _userService = usrSvc;
        }

        public IViewComponentResult Invoke()
        {
             bool isAssignable = (bool)HttpContext.Items[CorrectiveActionsConstants.IS_ASSIGNABLE_KEY];

            if (isAssignable)
            {
                UserViewModel qmsUser = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_KEY);
                List<User> usersByOrgList = _userService.RetrieveUsersByOrganizationId(qmsUser.OrgId);

                string assignedToUserId = (string)HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY];                  
                ViewBag.AssignedToUserItems = new SelectList(usersByOrgList, "UserId", "DisplayName", assignedToUserId);
            }
            
            return View();
        }
    }
}