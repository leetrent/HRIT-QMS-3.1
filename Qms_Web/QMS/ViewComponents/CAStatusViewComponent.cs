using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CAStatusViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}