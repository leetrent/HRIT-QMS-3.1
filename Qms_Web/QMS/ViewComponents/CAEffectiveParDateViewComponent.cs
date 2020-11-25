using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CAEffectiveParDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}