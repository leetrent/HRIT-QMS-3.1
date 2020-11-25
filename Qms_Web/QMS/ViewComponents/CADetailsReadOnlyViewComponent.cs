using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CADetailsReadOnlyViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}