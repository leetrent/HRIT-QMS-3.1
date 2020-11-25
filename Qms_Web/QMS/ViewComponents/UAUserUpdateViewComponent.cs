using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class UAUserUpdateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}