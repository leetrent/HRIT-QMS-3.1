using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CADetailsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}