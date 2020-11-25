using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.ViewModels;
using QMS.Constants;

namespace QMS.ViewComponents
{
	public class CANatureOfActionViewComponent : ViewComponent
	{
        private readonly IReferenceService _referenceService;

        public CANatureOfActionViewComponent(IReferenceService refSvc)
        {
            _referenceService = refSvc;
        }

		public IViewComponentResult Invoke()
        {
            string selectedNOACode = (string)HttpContext.Items[CorrectiveActionsConstants.NOA_CODE_KEY];
            		
			IQueryable<NatureOfAction> natureOfActions = _referenceService.RetrieveNatureOfActions();	
			ViewBag.NoaSelectItems = new SelectList(natureOfActions, "NoaCode", "SelectOptionText", selectedNOACode);
			
            return View();
        }
	}
}