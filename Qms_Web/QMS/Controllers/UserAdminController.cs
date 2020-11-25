using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Extensions;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.Helpers;

namespace QMS.Controllers
{
    public class UserAdminController : Controller
    {
        private readonly IUserService           _userService;
        private readonly IRoleService           _roleService;
        private readonly IPermissionService     _permissionService;
        private readonly IOrganizationService   _organizationService;
        private readonly IUserAdminHelper       _userAdminHelper;

        public UserAdminController(IUserService usrSvc, IRoleService roleSvc, IPermissionService permSvc, IOrganizationService orgSvc, IUserAdminHelper uaHelper)
        {
            _userService        = usrSvc;
            _roleService        = roleSvc;
            _permissionService  = permSvc;
            _organizationService = orgSvc;
            _userAdminHelper    = uaHelper;
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
                                .Append("][UserAdminController][Index][HttpGet] => ")
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

            Console.WriteLine(logSnippet + $"(qmsUserVM.IsSysAdmin): {qmsUserVM.IsSysAdmin}");
            Console.WriteLine(logSnippet + $"(qmsUserVM)...........: {qmsUserVM}");

            if (qmsUserVM.IsSysAdmin == false) 
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            UserAdminViewModel userAdminVM = HttpContext.Session.GetObject<UserAdminViewModel>(UserAdminConstants.USER_ADMIN_VIEW_MODEL);
            Console.WriteLine(logSnippet + $"(userAdminVM == null): {userAdminVM == null}");

            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();

            if (userAdminVM == null)
            {
                userAdminVM = new UserAdminViewModel
                {
                    ShowAlert                       = false,
                    ShowUserForm                    = false,
                    UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER,
                    UserNavItemNavLink              = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    UserTabPadFade                  = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                    ActiveRoleNavItemNavLink        = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActiveRoleTabPadFade            = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                    ActivePermissionNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActivePermissionTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };
  
                // ORGANIZATIONS
			    ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");

                // POTENTIAL MANAGERS
                List<User> allActiveUsers = _userService.RetrieveActiveUsers();
                ViewBag.PotentialManagers = new SelectList(activeOrganizations, "UserId", "DisplayLabel");
            }
            else
            {
                Console.WriteLine(logSnippet + $"(userAdminVM.User == null): {userAdminVM.User == null}");
                Console.WriteLine(logSnippet + $"(userAdminVM.User): {userAdminVM.User}");

                if ( userAdminVM.User != null)
                {
                    // ORGANIZATIONS
                    if (userAdminVM.User.OrgId.HasValue)
                    {
                        ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel", userAdminVM.User.OrgId);
                    }
                    else
                    {
                         ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");
                    }

                    // POTENTIAL MANAGERS
                    List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userAdminVM.User.OrgId.Value);
                    if (userAdminVM.User.ManagerId.HasValue)
                    {
                        ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userAdminVM.User.ManagerId);
                    }
                    else
                    {
                        ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
                    }
                }
            }

            userAdminVM.Permissions = _permissionService.RetrieveActivePermissions();
            userAdminVM.Roles       = _roleService.RetrieveAllRoles();

            userAdminVM.ActiveRoles     = new List<Role>();
            userAdminVM.InactiveRoles   = new List<Role>();

            foreach (Role role in userAdminVM.Roles)
            {
                if (role.IsActive == true)
                {
                    userAdminVM.ActiveRoles.Add(role);
                }
                else
                {
                     userAdminVM.InactiveRoles.Add(role);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // PERMISSION CHECKBOXES FOR EACH ROLE
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            _userAdminHelper.ProcessPermissionCheckboxesForAllRoles(userAdminVM.Roles, userAdminVM.Permissions);
        
            HttpContext.Session.Remove(UserAdminConstants.USER_ADMIN_VIEW_MODEL);
            return View(userAdminVM);
        }
    }
}