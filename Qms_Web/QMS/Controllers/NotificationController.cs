using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QMS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService ntfSvc)
        {
            _notificationService = ntfSvc;
        }

        [HttpGet]
        public IActionResult GoToWorkItem(int id)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][NotificationController][HttpGet][GoToWorkItem] => ")
                    .ToString();
           
            WorkItemType wiType = _notificationService.MarkAsRead(id);

            Console.WriteLine(logSnippet + $"(notificationId)........: {id}");
            Console.WriteLine(logSnippet + $"(wiType.MethodName).....: {wiType.MethodName}");
            Console.WriteLine(logSnippet + $"(wiType.WorkItemId).: {wiType.WorkItemId}");

            return RedirectToAction(wiType.MethodName, wiType.ControllerName, new{@id = wiType.WorkItemId} );
        }
/*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead(string notificationIdString)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][NotificationController][HttpPost][MarkAllAsRead] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(notificationIdString): '{notificationIdString}'");
            string[] notificationIdArray = notificationIdString.Split(',');
 
            foreach (string notificationId in notificationIdArray)
            {
                Console.WriteLine(logSnippet + $"(notificationId): '{notificationId}'");
                //notificationSvc.Delete(Int32.Parse(notificationId));
                _notificationService.MarkAsRead(Int32.Parse(notificationId));
            }           

            return RedirectToAction("Index", "Home");
        }
*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead(string notificationIdString)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][NotificationController][HttpPost][MarkAllAsRead] => ")
                    .ToString();

            string[] notificationIdStringArray  = notificationIdString.Split(',');
            int[]    notificationIdIntArray     = Array.ConvertAll(notificationIdStringArray, int.Parse);

            Console.WriteLine(logSnippet + $"(notificationIdString)............: '{notificationIdString}'");
            Console.WriteLine(logSnippet + $"(notificationIdStringArray.Length): '{notificationIdStringArray.Length}'");
            Console.WriteLine(logSnippet + $"(notificationIdIntArray.Length)...: '{notificationIdIntArray.Length}'");

            _notificationService.MarkAsRead(notificationIdIntArray);         

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAll(string notificationIdString)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][NotificationController][HttpPost][DeleteAll] => ")
                    .ToString();

            string[] notificationIdStringArray  = notificationIdString.Split(',');
            int[]    notificationIdIntArray     = Array.ConvertAll(notificationIdStringArray, int.Parse);

            Console.WriteLine(logSnippet + $"(notificationIdString)............: '{notificationIdString}'");
            Console.WriteLine(logSnippet + $"(notificationIdStringArray.Length): '{notificationIdStringArray.Length}'");
            Console.WriteLine(logSnippet + $"(notificationIdIntArray.Length)...: '{notificationIdIntArray.Length}'");

            _notificationService.Delete(notificationIdIntArray);        

            return RedirectToAction("Index", "Home");
        }
    }
}