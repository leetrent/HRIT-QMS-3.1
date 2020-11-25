using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Extensions;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.Helpers;
using QMS.Validators;

namespace QMS.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IPermissionValidator _permissionValidator;

        public PermissionController(IPermissionService permSvc, IPermissionValidator permVal)
        {
            _permissionService = permSvc;
            _permissionValidator = permVal;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][PermissionController][Index][HttpGet] => ")
                                .ToString();

            /////////////////////////////////////////////////////////////////////////////////////////
            // QMS User
            /////////////////////////////////////////////////////////////////////////////////////////
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM == null)
            {
                Console.WriteLine(logSnippet + "QMS User NOT IN SESSION, redirecting to [LoginConroller][LogoutAsync]");
                return RedirectToAction("LogoutAsync", "Login");
            }

            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if (qmsUserVM.CanRetrievePermission == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            PermissionViewModel permissionVM = HttpContext.Session.GetObject<PermissionViewModel>(UserAdminConstants.PERMISSION_VIEW_MODEL);
            Console.WriteLine(logSnippet + $"(permissionVM == null): {permissionVM == null}");

            if (permissionVM == null)
            {
                permissionVM = new PermissionViewModel
                {
                    ShowAlert                       = false,
                    UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_PERMISSION,
                    ActivePermissionNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActivePermissionTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };
            }
       
            HttpContext.Session.Remove(UserAdminConstants.PERMISSION_VIEW_MODEL);
            return View(permissionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePermission(string permissionCode, string permissionLabel)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][PermissionController][CreatePermission][HttpPost] => ")
                    .ToString();

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
         
            Console.WriteLine(logSnippet + $"(permissionCode)...: '{permissionCode}'");
            Console.WriteLine(logSnippet + $"(permissionLabel)..: '{permissionLabel}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null)..: {qmsUser == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanCreatePermission == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            PermissionViewModel permissionVM = new PermissionViewModel
            {
                ShowAlert = true,
                UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_PERMISSION,
                PermissionNavItemNavLink        = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                PermissionTabPadFade            = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActivePermissionNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActivePermissionTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            (bool isValid, string[] errorMessages) = _permissionValidator.CreatePermissionValuesAreValid(permissionCode, permissionLabel);
            if (isValid == false)
            {
                StringBuilder sb = new StringBuilder("<ul>");
                foreach (string errMsg in errorMessages)
                {
                    sb.Append("<li>").Append(errMsg).Append("</li>");
                }
                sb.Append("</ul>");

                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = sb.ToString();

                HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
                return RedirectToAction("Index", "Permission");
            }

            try
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO SAVE NEWLY CREATED PERMISSION TO DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                int permissionId = _permissionService.CreatePermission(permissionCode, permissionLabel, qmsUser);
                /////////////////////////////////////////////////////////////////////////////////////////////////
                
                permissionVM.AlertType    = UserAdminConstants.AlertTypeConstants.SUCCESS;
                permissionVM.AlertMessage = $"Permission successfully created: ['{permissionId}', '{permissionCode}', '{permissionLabel}']";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = $"Create new permission failed.";
            }
            
            HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
            return RedirectToAction("Index", "Permission");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePermission(int permissionId, string permissionLabel)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][PermissionController][UpdatePermission][HttpPost] => ")
                    .ToString();

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(permissionId).....: '{permissionId}'");
            Console.WriteLine(logSnippet + $"(permissionLabel)..: '{permissionLabel}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null)..: {qmsUser == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanUpdatePermission == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            
            PermissionViewModel permissionVM = new PermissionViewModel
            {
                ShowAlert = true,
                UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_PERMISSION,
                PermissionNavItemNavLink        = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                PermissionTabPadFade            = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActivePermissionNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActivePermissionTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            (bool isValid, string errMsg) = _permissionValidator.UpdatePermissionValuesAreValid(permissionId, permissionLabel);
            if (isValid == false)
            {
                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = $"<ul><li>{errMsg}</li></ul>";

                HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
                return RedirectToAction("Index", "Permission");
            }

            try
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO SAVE UPDATED PERMISSION TO DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                _permissionService.UpdatePermission(permissionId, permissionLabel, qmsUser);
                /////////////////////////////////////////////////////////////////////////////////////////////////

                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                permissionVM.AlertMessage = $"Permission successfully updated: ['{permissionId}', '{permissionLabel}']";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = $"Update to permission '{permissionId}' failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
            return RedirectToAction("Index", "Permission");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeactivatePermission(int permissionId)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][PermissionController][DeactivatePermission][HttpPost] => ")
                    .ToString();

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(permissionId).....: '{permissionId}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null)..: {qmsUser == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanDeactivatePermission == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            PermissionViewModel permissionVM = new PermissionViewModel
            {
                ShowAlert = true,
                UserAdminModule                     = UserAdminConstants.UserAdminModuleConstants.INACTIVE_PERMISSION,
                PermissionNavItemNavLink            = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                PermissionTabPadFade                = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                InactivePermissionNavItemNavLink    = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                InactivePermissionTabPadFade        = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            try
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO DEACTIVATE PERMISSION in DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                _permissionService.DeactivatePermission(permissionId, qmsUser);
                /////////////////////////////////////////////////////////////////////////////////////////////////

                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                permissionVM.AlertMessage = $"Permission {permissionId} has been successfully deactivated.";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = $"Attempt to deactivate permission '{permissionId}' failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
            return RedirectToAction("Index", "Permission");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReactivatePermission(int permissionId)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][PermissionController][ReactivatePermission][HttpPost] => ")
                    .ToString();

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(permissionId).....: '{permissionId}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null)..: {qmsUser == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanReactivatePermission == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            PermissionViewModel permissionVM = new PermissionViewModel
            {
                ShowAlert = true,
                UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_PERMISSION,
                PermissionNavItemNavLink        = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                PermissionTabPadFade            = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActivePermissionNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActivePermissionTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            try
            {
                ////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO REACTIVATE PERMISSION in DATABASE
                ////////////////////////////////////////////////////////////////
                _permissionService.ReactivatePermission(permissionId, qmsUser);
                ///////////////////////////////////////////////////////////////

                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                permissionVM.AlertMessage = $"Permission {permissionId} has been successfully reactivated.";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                permissionVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                permissionVM.AlertMessage = $"Attempt to reactivate permission '{permissionId}' failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.PERMISSION_VIEW_MODEL, permissionVM);
            return RedirectToAction("Index", "Permission");
        }
    }
}