using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace QMS.ViewComponents
{
    public class CACommentListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
              return View();
        }
    }
}