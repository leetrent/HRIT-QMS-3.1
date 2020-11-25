using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class UAUserCreateUpdateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}