using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class UARolesActiveViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}