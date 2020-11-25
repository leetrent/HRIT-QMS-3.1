using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.Services;
using QmsCore.UIModel;
using QmsCore.QmsException;
using QMS.Extensions;
using QMS.Constants;
using QMS.ViewModels;
using QMS.Utils;

namespace QMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUAUserUtil _userUtil;
        private readonly IOrganizationService _organizationService;

        public UserController(IUserService usrSvc, IUAUserUtil usrUtil, IOrganizationService orgSvc)
        {
            _userService = usrSvc;
            _userUtil = usrUtil;
            _organizationService = orgSvc;
        }

        [HttpGet]
        public IActionResult Index()
        {
            /////////////////////////////////////////////////////////////////////////////////////////
            // AUTHENTICATION CHECK
            /////////////////////////////////////////////////////////////////////////////////////////

            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Index][HttpGet] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM == null)
            {
                Console.WriteLine(logSnippet + "QMS User NOT IN SESSION, redirecting to [LoginConroller][LogoutAsync]");
                return RedirectToAction("LogoutAsync", "Login");
            }

            Console.WriteLine(logSnippet + $"(qmsUserVM.IsSysAdmin): {qmsUserVM.IsSysAdmin}");
            Console.WriteLine(logSnippet + $"(qmsUserVM)...........: {qmsUserVM}");

            /////////////////////////////////////////////////////////////////////////////////////////
            // AUTHORIZATION CHECK
            /////////////////////////////////////////////////////////////////////////////////////////
            if (qmsUserVM.CanRetrieveUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            // BEGIN USER ADMIN LOGIC
            /////////////////////////////////////////////////////////////////////////////////////////
            UAUserViewModel userVM = HttpContext.Session.GetObject<UAUserViewModel>(UserAdminConstants.UA_USER_VIEW_MODEL);
            Console.WriteLine(logSnippet + $"(userVM == null): {userVM == null}");

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // RENDER CREATE USER FORM
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            if (userVM != null && userVM.ShowCreateUserForm == true)
            {
                // ORGANIZATIONS
                List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
                ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");

                // POTENTIAL MANAGERS
                //List<User> allActiveUsers = _userService.RetrieveActiveUsers();
                List<User> usersInOrg = new List<User>();
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");

                HttpContext.Session.Remove(UserAdminConstants.UA_USER_VIEW_MODEL);
                return View(userVM);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // RENDER UPDATE USER FORM
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            if (userVM != null && userVM.ShowUpdateUserForm == true)
            {
                // ORGANIZATIONS
                List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
                ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel", userVM.OrgId);

                // POTENTIAL MANAGERS
                List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userVM.OrgId.Value);
                if (userVM.ManagerId.HasValue)
                {
                    ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userVM.ManagerId);
                }
                else
                {
                    ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
                }

                HttpContext.Session.Remove(UserAdminConstants.UA_USER_VIEW_MODEL);
                return View(userVM);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // RENDER ALERT, ADVISING OF SUCCESSFUL USER CREATE OR UPDATE TRANSACTION
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            if (userVM != null && userVM.ShowAlert == true)
            {
                HttpContext.Session.Remove(UserAdminConstants.UA_USER_VIEW_MODEL);
                return View(userVM);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // RENDER SEARCH USER FORM
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            userVM = new UAUserViewModel();
            userVM.SearchUserSuccessful = false;
            userVM.ShowUpdateUserForm = false;
            userVM.ShowCreateUserForm = false;
            userVM.AspAction = null;
            userVM.ShowAlert = false;
            userVM.AlertMessage = null;

            userVM.UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER;
            userVM.UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE;
            userVM.UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE;

            return View(userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string userSearchEmailAddress)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserController][Search][HttpPost] => ")
                    .ToString();

            User qmsUser = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(userSearchEmailAddress)...: '{userSearchEmailAddress}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null)..: {qmsUser == null}");
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanRetrieveUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            UAUserViewModel userVM = null;
            try
            {
                User dbUser = _userService.RetrieveByEmailAddress(userSearchEmailAddress);
                Console.WriteLine(logSnippet + $"(dbUser == null): {dbUser == null}");

                userVM = _userUtil.MapToViewModel(dbUser);
                Console.WriteLine(logSnippet + $"(userVM == null): {userVM == null}");

                userVM.SearchUserSuccessful = true;
                userVM.ShowUpdateUserForm = true;
                userVM.ShowCreateUserForm = false;
                userVM.AspAction = "Update";
                userVM.SubmitButtonLabel = "Update";
                userVM.CardHeader = "Update QMS User:";
                userVM.ShowAlert = false;
                userVM.AlertMessage = null;

                userVM.UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER;
                userVM.UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE;
                userVM.UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE;
            }
            catch (UserNotFoundException unfe)
            {
                Console.WriteLine(logSnippet + unfe.Message);

                userVM = new UAUserViewModel();
                _userUtil.PopulateCheckboxRolesForCreateUser(userVM);
                userVM.SearchUserSuccessful = false;
                userVM.ShowUpdateUserForm = false;
                userVM.ShowCreateUserForm = true;
                userVM.AspAction = "Create";
                userVM.SubmitButtonLabel = "Create";
                userVM.CardHeader = "Create QMS User:";
                userVM.ShowAlert = true;
                userVM.AlertMessage = $"'{userSearchEmailAddress}' not found.";
            }

            HttpContext.Session.SetObject(UserAdminConstants.UA_USER_VIEW_MODEL, userVM);
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("ManagerId, OrgId, EmailAddress, DisplayName")]
            UAUserViewModel userVM, string[] selectedRoleIdsForUser)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserController][Create][HttpPost] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(userVM.OrgId).................: {userVM.OrgId}");
            Console.WriteLine(logSnippet + $"(userVM.ManagerId).............: {userVM.ManagerId}");
            Console.WriteLine(logSnippet + $"(userVM.EmailAddress)..........: {userVM.EmailAddress}");
            Console.WriteLine(logSnippet + $"(userVM.DisplayName)...........: {userVM.DisplayName}");
            Console.WriteLine(logSnippet + $"(selectedRoleIdsForUser.Length): {selectedRoleIdsForUser.Length}");
            Console.WriteLine(logSnippet + $"(ModelState.IsValid)...........: {ModelState.IsValid}");

            foreach (string selectedRoleIdForUser in selectedRoleIdsForUser)
            {
                Console.WriteLine(logSnippet + $"(selectedRoleIdForUser): {selectedRoleIdForUser}");
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // User's Role Selection Validation
            ///////////////////////////////////////////////////////////////////////////////////
            if (selectedRoleIdsForUser == null || selectedRoleIdsForUser.Length == 0)
            {
                Console.WriteLine(logSnippet + "No roles selected for create user. Adding a model error.");
                ModelState.AddModelError(string.Empty, "Please select at least one role.");
            }

            Console.WriteLine(logSnippet + $"(ModelState.IsValid): {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                userVM.ShowUpdateUserForm = false;
                userVM.ShowCreateUserForm = false;
                userVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                userVM.ShowAlert = true;
                userVM.AlertMessage = $"New QMS User has been successfully created for: '{userVM.DisplayName} - [{userVM.EmailAddress}]'.";

                HttpContext.Session.SetObject(UserAdminConstants.UA_USER_VIEW_MODEL, userVM);
                return RedirectToAction("Index", "User");
            }
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update (
            [Bind("UserId, ManagerId, OrgId, EmailAddress, DisplayName")]
            UAUserViewModel userVM, string[] selectedRoleIdsForUser)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserController][Update][HttpPost] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(userVM.UserId)................: {userVM.UserId}");
            Console.WriteLine(logSnippet + $"(userVM.OrgId).................: {userVM.OrgId}");
            Console.WriteLine(logSnippet + $"(userVM.ManagerId).............: {userVM.ManagerId}");
            Console.WriteLine(logSnippet + $"(userVM.EmailAddress)..........: {userVM.EmailAddress}");
            Console.WriteLine(logSnippet + $"(userVM.DisplayName)...........: {userVM.DisplayName}");
            Console.WriteLine(logSnippet + $"(selectedRoleIdsForUser.Length): {selectedRoleIdsForUser.Length}");
            Console.WriteLine(logSnippet + $"(ModelState.IsValid)...........: {ModelState.IsValid}");

            foreach (string selectedRoleIdForUser in selectedRoleIdsForUser)
            {
                Console.WriteLine(logSnippet + $"(selectedRoleIdForUser): {selectedRoleIdForUser}");
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // User's Role Selection Validation
            ///////////////////////////////////////////////////////////////////////////////////
            if (selectedRoleIdsForUser == null || selectedRoleIdsForUser.Length == 0)
            {
                Console.WriteLine(logSnippet + "No roles selected for update user. Adding a model error.");
                ModelState.AddModelError(string.Empty, "Please select at least one role.");
            }

            Console.WriteLine(logSnippet + $"(ModelState.IsValid): {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                userVM.ShowUpdateUserForm = false;
                userVM.ShowCreateUserForm = false;
                userVM.AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS;
                userVM.ShowAlert = true;
                userVM.AlertMessage = $"QMS user identified by '{userVM.DisplayName} - [{userVM.EmailAddress}]' has been successfully updated.";

                HttpContext.Session.SetObject(UserAdminConstants.UA_USER_VIEW_MODEL, userVM);
                return RedirectToAction("Index", "User");
            }
            return View();
        }
    }
}