using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CADateSubmittedViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}