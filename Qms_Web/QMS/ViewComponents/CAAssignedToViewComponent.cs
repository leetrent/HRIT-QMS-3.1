using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CAAssignedToViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}