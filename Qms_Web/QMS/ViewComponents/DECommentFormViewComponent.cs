using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class DECommentFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}