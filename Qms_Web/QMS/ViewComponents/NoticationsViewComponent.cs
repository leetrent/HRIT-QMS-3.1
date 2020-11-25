using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.Extensions;
using QMS.Constants;
using QMS.ViewModels;

namespace QMS.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationsViewComponent(INotificationService ntfSvc)
        {
            _notificationService = ntfSvc;
        }
        
        public IViewComponentResult Invoke()
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                 return View();
            }

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            if ( qmsUserVM.CanViewNotifications == false )
            {
                return View();
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][NotificationsViewComponent][Invoke] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");
            
            List<Notification> svcNotificationList = _notificationService.RetrieveUserNotifications(qmsUserVM.UserId, false);
            Console.WriteLine(logSnippet + $"(svcNotificationList == null): {svcNotificationList == null}");
            
            StringBuilder sb = new StringBuilder();
            if ( svcNotificationList != null)
            {
                Console.WriteLine(logSnippet + $"(svcNotificationList.Count): {svcNotificationList.Count}");
                // foreach (var notification in svcNotificationList)
                // {
                //     Console.WriteLine(logSnippet + $"(notification.NotificationId): {notification.NotificationId}");
                //     Console.WriteLine(logSnippet + $"(notification.WorkitemId)....: {notification.WorkitemId}");
                //     Console.WriteLine(logSnippet + $"(notification.HasBeenRead)...: {notification.HasBeenRead}");
                // }
                int count = 0;
                foreach (var notification in svcNotificationList)
                {
                    if ( count > 0) {sb.Append(",");}
                    sb.Append(notification.NotificationId);
                    count++;
                }
            }

            if ( svcNotificationList == null)
            {
                svcNotificationList = new List<Notification>();
            }
            
            Console.WriteLine(logSnippet + $"(svcNotificationList == null): {svcNotificationList == null}");
            
            ViewBag.NotificationList = svcNotificationList;
            ViewBag.NotificationIdString = sb.ToString();

            return View();
        }
    }
}