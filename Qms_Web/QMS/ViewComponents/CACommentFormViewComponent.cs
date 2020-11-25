using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CACommentFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}