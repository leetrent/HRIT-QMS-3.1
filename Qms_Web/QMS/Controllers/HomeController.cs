using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Extensions;

namespace QMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public HomeController(IConfiguration configuration, INotificationService ntfSvc, IEmailService emailSvc)
        {
            _config = configuration;
            _notificationService = ntfSvc;
            _emailService = emailSvc;
         }

        public IActionResult UnauthorizedAccess()
        {
            return View();
        }

        public IActionResult Index(string mk = null)
        {
             string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][HomeController][HttpGet][Index] => ")
                    .ToString();

            if (User.Identity.IsAuthenticated == false) 
            {
                //return View();
                 return RedirectToAction("Warning", "Home");
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            // QMS User
            /////////////////////////////////////////////////////////////////////////////////////////
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            if (qmsUserVM == null)
            {
                Console.WriteLine(logSnippet + "QMS User NOT IN SESSION, redirecting to [LoginConroller][LogoutAsync]");
                return RedirectToAction("LogoutAsync", "Login");
            }           
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            //ViewData["QMS_ROLE_LABELS"] = $"{qmsUserVM.RoleLabels}";
            ViewData["QMS_ORG"] =  $"{qmsUserVM.OrgLabel}";

            ViewBag.UserRoleLabels = qmsUserVM.RoleLabels;

            /////////////////////////////////////////////////////////////////////////////////////////
            // ModuleMenuItem
            /////////////////////////////////////////////////////////////////////////////////////////
            List<ModuleMenuItem> moduleMenuItems = HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);
            Console.WriteLine(logSnippet + "(moduleMenuItems == null): " + (moduleMenuItems == null));

            if (moduleMenuItems == null)
            {
                Console.WriteLine(logSnippet + "List<ModuleMenuItem> moduleMenuItems NOT IN SESSION, redirecting to [LoginConroller][LogoutAsync]");
                return RedirectToAction("LogoutAsync", "Login");;
            }

            ViewData[MiscConstants.HOME_PAGE_ALERT_MESSAGE_KEY] = null;
            if ( String.IsNullOrEmpty(mk) == false && String.IsNullOrWhiteSpace(mk) == false )
            {
                if ( HttpContext.Session.GetObject<string>(mk) != null)
                {
                    ViewData[MiscConstants.HOME_PAGE_ALERT_MESSAGE_KEY] = HttpContext.Session.GetObject<string>(mk);
                    HttpContext.Session.SetObject(mk, null);
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][HomeController][Error] => ")
                                .ToString();

            bool showErrorDetails = false;
            try
            {
                showErrorDetails = Convert.ToBoolean(_config[AppSettingsConstants.SHOW_ERROR_DETAILS]);
            }
            catch (FormatException)
            {
                showErrorDetails = false;
            }

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Console.WriteLine(logSnippet + $"(AppSettings.ShowErrorDetails)......: '{_config[AppSettingsConstants.SHOW_ERROR_DETAILS]}'");
            Console.WriteLine(logSnippet + $"(showErrorDetails)..................: '{showErrorDetails}'");
            Console.WriteLine(logSnippet + $"(Activity.Current?.Id)..............: '{Activity.Current?.Id}'");
            Console.WriteLine(logSnippet + $"(HttpContext.TraceIdentifier).......: '{HttpContext.TraceIdentifier}'");
            Console.WriteLine(logSnippet + $"(exceptionHandlerPathFeature).......: '{exceptionHandlerPathFeature}'");
            Console.WriteLine(logSnippet + $"(exceptionHandlerPathFeature?.Error): '{exceptionHandlerPathFeature?.Error}'");

            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                ShowErrorDetails = showErrorDetails,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                QmsException = exceptionHandlerPathFeature?.Error
            };

            try
            {
                StringBuilder subject = new StringBuilder("QMS Exception Encountered");
                string userEmailAddress = HttpContext.Session.GetObject<string>(MiscConstants.USER_EMAIL_ADDRESS);
                if (String.IsNullOrEmpty(userEmailAddress) == false 
                        && String.IsNullOrWhiteSpace(userEmailAddress) == false)
                {
                    subject.Append($" - {userEmailAddress}");
                }
                else
                {
                    subject.Append("- User Email Address IS NULL");
                }

                string message  = (errorViewModel.QmsException == null)
                                ? ("Exception IS NULL")
                                : (errorViewModel.QmsException.ToString());     
                _notificationService.SendEmail(subject.ToString(), message);
            }
            catch(Exception exc)
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + $"BEGIN: EXCEPTION MESSAGE");
                Console.WriteLine(exc.Message);
                Console.WriteLine(logSnippet + $"END: EXCEPTION MESSAGE");
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + $"BEGIN: EXCEPTION STACKTRACE");
                Console.WriteLine(exc.StackTrace);
                Console.WriteLine(logSnippet + $"END: EXCEPTION STACKTRACE");
                Console.WriteLine("================================================================================");                
            }

            return View(errorViewModel);
        }
        public IActionResult Warning(string requestedUri)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][HomeController][Warning] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(requestedUri) '{requestedUri}'");
            Console.WriteLine(logSnippet + $"(Url.IsLocalUrl(requestedUri)) '{Url.IsLocalUrl(requestedUri)}'");

            if ( string.IsNullOrEmpty(requestedUri) == false
                    && string.IsNullOrWhiteSpace(requestedUri) == false)
                    //&& Url.IsLocalUrl(requestedUri) )
            {
                HttpContext.Session.SetObject(MiscConstants.REQUESTED_URI, requestedUri); 
            }
 
            HttpContext.Session.SetObject(UserConstants.WARNING_AGREED_TO, false); 
            return View();
        }

        public IActionResult WarningAgreedTo()
        {
            HttpContext.Session.SetObject(UserConstants.WARNING_AGREED_TO, true); 
            return RedirectToAction("LoginAsync", "Login");
        }

        [HttpGet]
        public IActionResult Help(string showEmailSentAlert = null)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][HomeController][Help][HttpGet] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(showEmailSentAlert): '{showEmailSentAlert}'");

            EmailViewModel emailVM = new EmailViewModel();
            emailVM.ShowAlert = false;
            try
            {
                emailVM.ShowAlert = Convert.ToBoolean(showEmailSentAlert);
            }
            catch (FormatException)
            {
                emailVM.ShowAlert = false;
            }
            Console.WriteLine(logSnippet + $"(emailVM.ShowAlert): '{emailVM.ShowAlert}'");

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM)........: {qmsUserVM}");
            
            if (qmsUserVM == null)
            {
                HttpContext.Session.SetObject(MiscConstants.REQUESTED_URI, "Home/Help"); 
                HttpContext.Session.SetObject(UserConstants.WARNING_AGREED_TO, true);
                return RedirectToAction("LoginAsync", "Login");
            }
            else
            {
                emailVM.Sender = qmsUserVM.EmailAddress;
            }

            return View(emailVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Help ( [Bind("Subject, Body")] EmailViewModel emailVM )
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][HomeController][Help][HttpPost] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(Sender)...: '{emailVM.Sender}'");
            Console.WriteLine(logSnippet + $"(Recipient): '{emailVM.Recipient}'");
            Console.WriteLine(logSnippet + $"(Subject)..: '{emailVM.Subject}'");
            Console.WriteLine(logSnippet + $"(Body).....: '{emailVM.Body}'");

            // string[] recipients = new string[2];
            // recipients[0] = "lee.trent@gsa.gov";
            // recipients[1] = "alfred.ortega@gsa.gov";

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM)........: {qmsUserVM}");
            
            if (qmsUserVM == null)
            {
                emailVM.Sender = "no-reply@gsa.gov";
            }
            else
            {
                emailVM.Sender = qmsUserVM.EmailAddress;
            }

            _emailService.SendEmail(emailVM.Sender, "qms.support@gsa.gov", emailVM.Subject, emailVM.Body);

            // return RedirectToAction("Index", "Home");
            return RedirectToAction(nameof(Help), new{ @showEmailSentAlert = Boolean.TrueString});
        }

        public IActionResult UserGuides()
        {
            return View();
        }
    }
}