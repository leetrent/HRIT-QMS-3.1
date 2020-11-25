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
using QMS.Validators;

namespace QMS.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService     _permissionService;
        private readonly IUserAdminHelper _userAdminHelper;
        private readonly IRoleValidator _roleValidator;

        public RoleController(IRoleService roleSvc, IPermissionService permSvc, IUserAdminHelper uaHelper, IRoleValidator roleVal)
        {
            _roleService = roleSvc;
            _permissionService = permSvc;
            _userAdminHelper = uaHelper;
            _roleValidator = roleVal;
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
                                .Append("][RoleController][Index][HttpGet] => ")
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

            if (qmsUserVM.CanRetrieveRole == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            RoleViewModel roleVM = HttpContext.Session.GetObject<RoleViewModel>(UserAdminConstants.ROLE_VIEW_MODEL);
            Console.WriteLine(logSnippet + $"(roleVM == null): {roleVM == null}");

            if (roleVM == null)
            {
                roleVM = new RoleViewModel
                {
                    ShowAlert                       = false,
                    UserAdminModule                 = UserAdminConstants.UserAdminModuleConstants.ACTIVE_ROLE,
                    ActiveRoleNavItemNavLink        = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActiveRoleTabPadFade            = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };
            }

            roleVM.ActiveRoles      = new List<Role>();
            roleVM.InactiveRoles    = new List<Role>();

            List<Role> allRoles = _roleService.RetrieveAllRoles();
            foreach (Role role in allRoles)
            {
                if (role.IsActive == true)
                {
                    roleVM.ActiveRoles.Add(role);
                }
                else
                {
                     roleVM.InactiveRoles.Add(role);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // PERMISSION CHECKBOXES FOR EACH ROLE
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            roleVM.Permissions = _permissionService.RetrieveActivePermissions();
            _userAdminHelper.ProcessPermissionCheckboxesForAllRoles(allRoles, roleVM.Permissions);
        
            HttpContext.Session.Remove(UserAdminConstants.ROLE_VIEW_MODEL);
            return View(roleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRole(string roleCode, string roleLabel, string[] selectedPermissions)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][RoleController][CreateRole][HttpPost] => ")
                    .ToString();

            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanCreateRole == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(roleCode).: '{roleCode}'");
            Console.WriteLine(logSnippet + $"(roleLabel): '{roleLabel}'");         
            Console.WriteLine(logSnippet + $"(selectedPermissions == null): '{selectedPermissions == null}'");

            foreach (string selectedPermissionId in selectedPermissions)
            {
                Console.WriteLine(logSnippet + $"(selectedPermissionId): '{selectedPermissionId}'");
            }

            RoleViewModel roleVM = new RoleViewModel
            {
                ShowAlert = true,
                UserAdminModule             = UserAdminConstants.UserAdminModuleConstants.ACTIVE_ROLE,
                RoleNavItemNavLink          = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                RoleTabPadFade              = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActiveRoleNavItemNavLink    = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActiveRoleTabPadFade        = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            (bool isValid, string[] errorMessages) = _roleValidator.CreateRoleValuesAreValid(roleCode, roleLabel, selectedPermissions);
            if (isValid == false)
            {
                StringBuilder sb = new StringBuilder("<ul>");
                foreach (string errMsg in errorMessages)
                {
                    sb.Append("<li>").Append(errMsg).Append("</li>");
                }
                sb.Append("</ul>");

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = sb.ToString();

                HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
                return RedirectToAction("Index", "Role");
            }

            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////
                // INSTANTIATE AND POPULATE UIModel class (Role)
                ///////////////////////////////////////////////////////////////////////////////////////
                Role role = _userAdminHelper.CreateNewRole(roleCode, roleLabel, selectedPermissions);
                ///////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO SAVE NEWLY CREATED ROLE (with permissions) TO DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                int roleId = _roleService.CreateRole(role);
                roleVM.JustEditedRoleId = roleId;
                /////////////////////////////////////////////////////////////////////////////////////////////////

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                roleVM.AlertMessage = $"New Role successfully created: ['{roleId}', '{role.RoleCode}', '{role.RoleLabel}']";
            }
            catch (Exception exc)
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + exc.Message);
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + exc.StackTrace);
                Console.WriteLine("================================================================================");

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = $"Create new role failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
            return RedirectToAction("Index", "Role");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(string roleId, string roleCode, string roleLabel, string[] roleUpdatePermissionsIds)
        {

            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][RoleController][UpdateRole][HttpPost] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanUpdateRole == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(roleId)...: '{roleId}'");
            Console.WriteLine(logSnippet + $"(roleCode).: '{roleCode}'");
            Console.WriteLine(logSnippet + $"(roleLabel): '{roleLabel}'");
            Console.WriteLine(logSnippet + $"(roleUpdatePermissionsIds == null): '{roleUpdatePermissionsIds == null}'");

            foreach (string roleUpdatePermissionsId in roleUpdatePermissionsIds)
            {
                Console.WriteLine(logSnippet + $"(roleUpdatePermissionsId): '{roleUpdatePermissionsId}'");
            }

            RoleViewModel roleVM = new RoleViewModel
            {
                ShowAlert = true,
                UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_ROLE,
                RoleNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                RoleTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActiveRoleNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActiveRoleTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE
            };


            (bool isValid, string[] errorMessages) = _roleValidator.UpdateRoleValuesAreValid(Int32.Parse(roleId), roleLabel, roleUpdatePermissionsIds);
            if (isValid == false)
            {
                StringBuilder sb = new StringBuilder("<ul>");
                foreach (string errMsg in errorMessages)
                {
                    sb.Append("<li>").Append(errMsg).Append("</li>");
                }
                sb.Append("</ul>");

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = sb.ToString();

                HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
                return RedirectToAction("Index", "Role");
            }

            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////
                // INSTANTIATE AND POPULATE UIModel class (Role)
                ///////////////////////////////////////////////////////////////////////////////////////
                Role role = _userAdminHelper.CreateNewRole(roleCode, roleLabel, roleUpdatePermissionsIds, roleId);
                roleVM.JustEditedRoleId = role.ID;
                ///////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL PERMISSION SERVICE TO SAVE NEWLY CREATED ROLE (with permissions) TO DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                int nbrOfRowsUpdated = _roleService.UpdateRole(role);
                /////////////////////////////////////////////////////////////////////////////////////////////////

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                roleVM.AlertMessage = $"Role successfully updated: ['{roleId}', '{role.RoleCode}', '{role.RoleLabel}']";
            }
            catch (Exception exc)
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + exc.Message);
                Console.WriteLine("================================================================================");
                Console.WriteLine(logSnippet + exc.StackTrace);
                Console.WriteLine("================================================================================");

                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = $"Update role failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
            return RedirectToAction("Index", "Role");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeactivateRole(int roleId)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][RoleController][DeactivateRole][HttpPost] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanDeactivateRole == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            Console.WriteLine(logSnippet + $"(roleId)...: '{roleId}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null): {qmsUser == null}");

            RoleViewModel roleVM = new RoleViewModel
            {
                ShowAlert = true,
                UserAdminModule             = UserAdminConstants.UserAdminModuleConstants.INACTIVE_ROLE,
                RoleNavItemNavLink          = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                RoleTabPadFade              = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                InactiveRoleNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                InactiveRoleTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            try
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL ROLE SERVICE TO DEACTIVATE ROLE in DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                _roleService.DeactivateRole(roleId);
                /////////////////////////////////////////////////////////////////////////////////////////////////

                roleVM.JustEditedRoleId = roleId;
                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                roleVM.AlertMessage = $"Role {roleId} has been successfully deactivated.";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = $"Attempt to deactivate role '{roleId}' failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
            return RedirectToAction("Index", "Role");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReactivateRole(int roleId)
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][RoleController][ReactivateRole][HttpPost] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanReactivateRole == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            Console.WriteLine(logSnippet + $"(roleId)...: '{roleId}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null): {qmsUser == null}");

            RoleViewModel roleVM = new RoleViewModel
            {
                ShowAlert = true,
                UserAdminModule             = UserAdminConstants.UserAdminModuleConstants.ACTIVE_ROLE,
                RoleNavItemNavLink          = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                RoleTabPadFade              = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActiveRoleNavItemNavLink  = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActiveRoleTabPadFade      = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            try
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////
                // CALL ROLE SERVICE TO DEACTIVATE ROLE in DATABASE
                /////////////////////////////////////////////////////////////////////////////////////////////////
                _roleService.ReactivateRole(roleId);
                /////////////////////////////////////////////////////////////////////////////////////////////////
                
                roleVM.JustEditedRoleId = roleId;
                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                roleVM.AlertMessage = $"Role {roleId} has been successfully reactivated.";
            }
            catch (Exception exc)
            {
                Console.WriteLine(logSnippet + exc.Message);
                roleVM.AlertType = UserAdminConstants.AlertTypeConstants.FAILURE;
                roleVM.AlertMessage = $"Attempt to reactivate role '{roleId}' failed.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.ROLE_VIEW_MODEL, roleVM);
            return RedirectToAction("Index", "Role");
        }
    }
}