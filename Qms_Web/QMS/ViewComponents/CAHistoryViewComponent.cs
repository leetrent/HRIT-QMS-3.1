using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CAHistoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}