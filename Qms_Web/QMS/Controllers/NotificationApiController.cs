using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;
using QMS.ApiModels;

namespace QMS.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationApiController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationApiController(INotificationService ntfSvc)
        {
            _notificationService = ntfSvc;
        }

        [HttpGet]
        public ActionResult<NotificationItem> GetNotification(int id)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][NotificationApiController][HttpGet][GetNotification] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(id): {id}");
            return new NotificationItem{ NotificationId = Convert.ToString(id) };
        }
        
        [HttpPost]
        public IActionResult PostNotification(NotificationItem itemParam)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][NotificationApiController][HttpPost][PostNotification] => ")
                                .ToString();
            
            Console.WriteLine(logSnippet + $"(itemParam == null).......: {itemParam == null}");
            Console.WriteLine(logSnippet + $"(itemParam.NotificationId): {itemParam.NotificationId}");
            
            Console.WriteLine(logSnippet + $"Calling NotificationService.Delete({itemParam.NotificationId})...");
            _notificationService.Delete(Int32.Parse(itemParam.NotificationId));
            Console.WriteLine(logSnippet + $"...Returning from  NotificationService.Delete({itemParam.NotificationId})");
            
            return CreatedAtAction(nameof(GetNotification), new { @id = itemParam.NotificationId } );
        } 
    }
}