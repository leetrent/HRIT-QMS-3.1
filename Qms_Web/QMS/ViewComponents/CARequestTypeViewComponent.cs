using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.ViewModels;

namespace QMS.ViewComponents
{
    public class CARequestTypeViewComponent : ViewComponent
    {
        private readonly IReferenceService _referenceService;

        public CARequestTypeViewComponent(IReferenceService refSvc)
        {
            _referenceService = refSvc;
        }

        public IViewComponentResult Invoke()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////
            // Action Types (single-select drop-down)
            //////////////////////////////////////////////////////////////////////////////////////////////
            IQueryable<ActionType> actionTypes = _referenceService.RetrieveActionTypes();
            ViewBag.ActionTypeItems = new SelectList(actionTypes, "Id", "Label");

            return View();
        }
    }
}