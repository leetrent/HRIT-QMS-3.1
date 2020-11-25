using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CASubmittedByViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}