using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.UIModel;
using QmsCore.Services;
using QmsCore.Model;
using QMS.Constants;
using QMS.Extensions;
using QMS.ViewModels;

namespace QMS.ViewComponents
{
    public class CAActionViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IReferenceService _referenceService;

        public CAActionViewComponent(IUserService usrSvc, IReferenceService refSvc)
        {
            _userService = usrSvc;
            _referenceService = refSvc;
        }

        public IViewComponentResult Invoke()
        {
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);

            List<Status> statusList = null;
            if ( HttpContext.Items[CorrectiveActionsConstants.CURRENT_STATUS_ID_KEY] == null)
            {
                statusList = _referenceService.RetrieveAvailableActionsList(StatusType.NONE, qmsUser.Organization);   
            }
            else
            {
                int currentStatusId = (int)HttpContext.Items[CorrectiveActionsConstants.CURRENT_STATUS_ID_KEY];
                 statusList = _referenceService.RetrieveAvailableActionsList(currentStatusId, qmsUser.Organization);
            }
            
            ViewBag.StatusTypeItems = new SelectList(statusList, "StatusId", "StatusLabel");

            return View();
        }
    }
}