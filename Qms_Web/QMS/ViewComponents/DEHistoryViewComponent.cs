using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class DEHistoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}