using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class UAUserCreateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}