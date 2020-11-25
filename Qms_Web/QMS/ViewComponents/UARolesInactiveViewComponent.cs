using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class UARolesInactiveViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}