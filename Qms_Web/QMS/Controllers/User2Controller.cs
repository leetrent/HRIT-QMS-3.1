using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.Utils;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Extensions;

namespace QMS.Controllers
{
    public class User2Controller : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserFormUtil _userFormUtil;
        private readonly IOrganizationService _organizationService;

        public User2Controller(IUserService usrSvc, IUserFormUtil usrFmUtil, IOrganizationService orgSvc)
        {
            _userService = usrSvc;
            _userFormUtil = usrFmUtil;
            _organizationService = orgSvc;
        }

        [HttpGet]
        public IActionResult Index()
        {
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

            if (qmsUserVM.CanRetrieveUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            UserListViewModel userListVM = HttpContext.Session.GetObject<UserListViewModel>(UserAdminConstants.USER_LIST_VIEW_MODEL);
            Console.WriteLine(logSnippet + $"(userListVM == null): {userListVM == null}");

            if (userListVM == null)
            {
                userListVM = new UserListViewModel
                {
                    ShowAlert = false,
                    UserAdminModule             = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER,
                    ActiveUserNavItemNavLink    = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActiveUserTabPadFade        = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };
            }

            HttpContext.Session.Remove(UserAdminConstants.USER_LIST_VIEW_MODEL);
            return View(userListVM);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: User/Create
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Create][HttpGet] => ")
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

            if (qmsUserVM.CanCreateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            UserFormViewModel userFormVM = new UserFormViewModel();
            userFormVM.Mutatatable          = true;
            userFormVM.Deactivatable        = false;
            userFormVM.Reactivatable        = false;
            userFormVM.AspAction            = "Create";
            userFormVM.SubmitButtonLabel    = "Create";
            userFormVM.CardHeader           = "Create QMS User:";
            userFormVM.Deactivatable        = false;
            _userFormUtil.PopulateCheckboxRolesForCreateUser(userFormVM);

            // ORGANIZATIONS
            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
            ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");

            // POTENTIAL MANAGERS
            List<User> usersInOrg = new List<User>();
            ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");

            return View(userFormVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("ManagerId, OrgId, EmailAddress, DisplayName")]
            UserFormViewModel userFormVM, string[] selectedRoleIdsForUser)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Create][HttpPost] => ")
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

            if (qmsUserVM.CanCreateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(userFormVM.OrgId).................: {userFormVM.OrgId}");
            Console.WriteLine(logSnippet + $"(userFormVM.ManagerId).............: {userFormVM.ManagerId}");
            Console.WriteLine(logSnippet + $"(userFormVM.EmailAddress)..........: {userFormVM.EmailAddress}");
            Console.WriteLine(logSnippet + $"(userFormVM.DisplayName)...........: {userFormVM.DisplayName}");
            Console.WriteLine(logSnippet + $"(ModelState.IsValid)...............: {ModelState.IsValid}");
            Console.WriteLine(logSnippet + $"(userFormVM.Length): {selectedRoleIdsForUser.Length}");
         
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
                Console.WriteLine(logSnippet + $"(qmsUserVM.UserId).: '{qmsUserVM.UserId}'");
                User submitter = _userService.RetrieveByUserId(qmsUserVM.UserId);
                Console.WriteLine(logSnippet + $"(submitter == null): '{submitter == null}'");
                Console.WriteLine(logSnippet + $"(submitter.UserId).: '{submitter.UserId}'");

                User userDB = _userFormUtil.MapToUIModelOnCreate(userFormVM, selectedRoleIdsForUser);
                int userId = _userService.CreateUser(userDB, submitter);

                UserListViewModel userListVM = new UserListViewModel
                {
                    ShowAlert = true,
                    AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS,
                    AlertMessage = $"New QMS User has been successfully created for: '{userFormVM.DisplayName} - [{userFormVM.EmailAddress}]'.",
                    UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER,
                    UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                    ActiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };

                HttpContext.Session.SetObject(UserAdminConstants.USER_LIST_VIEW_MODEL, userListVM);
                return RedirectToAction(nameof(Index));
            }


            //UserFormViewModel userFormVM = new UserFormViewModel();
            userFormVM.Mutatatable          = true;
            userFormVM.Deactivatable        = false;
            userFormVM.Reactivatable        = false;
            userFormVM.AspAction = "Create";
            userFormVM.SubmitButtonLabel = "Create";
            userFormVM.CardHeader = "Create QMS User:";
            _userFormUtil.PopulateCheckboxRolesForCreateUser(userFormVM);

            // ORGANIZATIONS
            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
            ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");

            // POTENTIAL MANAGERS
            if (userFormVM.ManagerId.HasValue && userFormVM.ManagerId.Value > 0)
            {
                List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userFormVM.OrgId.Value);
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userFormVM.ManagerId);
            }
            else
            {
                List<User> usersInOrg = new List<User>();
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
            }

            //return View(userFormVM);

            _userFormUtil.PopulateCheckboxRolesForUser(userFormVM, selectedRoleIdsForUser);

            return View(userFormVM);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: User/Update
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Update(int userId)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Update][HttpGet] => ")
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

            if (qmsUserVM.CanUpdateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(userId): {userId}");
            User userDB = _userService.RetrieveByUserId(userId);
            //User userDB = _userService.RetrieveByEmailAddress("testuser04@gsa.gov");
            Console.WriteLine(logSnippet + $"(userDB == null): {userDB == null}");

            UserFormViewModel userFormVM = _userFormUtil.MapToViewModel(userDB);
            Console.WriteLine(logSnippet + $"(userFormVM == null): {userFormVM == null}");

            userFormVM.Mutatatable          = true;
            userFormVM.Deactivatable        = true;
            userFormVM.Reactivatable        = false;
            userFormVM.AspAction            = "Update";
            userFormVM.SubmitButtonLabel    = "Update";
            userFormVM.CardHeader           = "Update QMS User:";
            //_userFormUtil.PopulateCheckboxRolesForCreateUser(userFormVM);

            // ORGANIZATIONS
            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
            ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel", userDB.OrgId);

            // POTENTIAL MANAGERS
            if (userFormVM.ManagerId.HasValue && userFormVM.ManagerId.Value > 0)
            {
                List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userFormVM.OrgId.Value);
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userFormVM.ManagerId);
            }
            else
            {
                List<User> usersInOrg = new List<User>();
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
            }

            return View(userFormVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(
            [Bind("UserId, ManagerId, OrgId, EmailAddress, DisplayName")]
            UserFormViewModel userFormVM, string[] selectedRoleIdsForUser)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Update][HttpPost] => ")
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

            if (qmsUserVM.CanUpdateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(userFormVM.UserId)................: {userFormVM.UserId}");
            Console.WriteLine(logSnippet + $"(userFormVM.OrgId).................: {userFormVM.OrgId}");
            Console.WriteLine(logSnippet + $"(userFormVM.ManagerId).............: {userFormVM.ManagerId}");
            Console.WriteLine(logSnippet + $"(userFormVM.EmailAddress)..........: {userFormVM.EmailAddress}");
            Console.WriteLine(logSnippet + $"(userFormVM.DisplayName)...........: {userFormVM.DisplayName}");
            Console.WriteLine(logSnippet + $"(ModelState.IsValid)...............: {ModelState.IsValid}");
            Console.WriteLine(logSnippet + $"(userFormVM.Length): {selectedRoleIdsForUser.Length}");

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
                Console.WriteLine(logSnippet + $"(qmsUserVM.UserId).: '{qmsUserVM.UserId}'");
                User submitter = _userService.RetrieveByUserId(qmsUserVM.UserId);
                Console.WriteLine(logSnippet + $"(submitter == null): '{submitter == null}'");
                Console.WriteLine(logSnippet + $"(submitter.UserId).: '{submitter.UserId}'");

                User userDB = _userFormUtil.MapToUIModelOnCreate(userFormVM, selectedRoleIdsForUser);
                _userService.UpdateUser(userDB, submitter);

                UserListViewModel userListVM = new UserListViewModel
                {
                    ShowAlert = true,
                    AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS,
                    AlertMessage = $"New QMS User has been successfully updated for: '{userFormVM.DisplayName} - [{userFormVM.EmailAddress}]'.",
                    UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER,
                    UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                    ActiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                    ActiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                };

                HttpContext.Session.SetObject(UserAdminConstants.USER_LIST_VIEW_MODEL, userListVM);
                return RedirectToAction(nameof(Index));
            }


            //UserFormViewModel userFormVM = new UserFormViewModel();
            //userFormVM.AspAction = "Update";
            //userFormVM.SubmitButtonLabel = "Update";
            //userFormVM.CardHeader = "Update QMS User:";

            userFormVM.Mutatatable          = true;
            userFormVM.Deactivatable        = true;
            userFormVM.Reactivatable        = false;
            userFormVM.AspAction            = "Update";
            userFormVM.SubmitButtonLabel    = "Update";
            userFormVM.CardHeader           = "Update QMS User:";

            _userFormUtil.PopulateCheckboxRolesForCreateUser(userFormVM);

            // ORGANIZATIONS
            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
            ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel");

            // POTENTIAL MANAGERS
            if (userFormVM.ManagerId.HasValue && userFormVM.ManagerId.Value > 0)
            {
                List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userFormVM.OrgId.Value);
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userFormVM.ManagerId);
            }
            else
            {
                List<User> usersInOrg = new List<User>();
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
            }

            //return View(userFormVM);

            _userFormUtil.PopulateCheckboxRolesForUser(userFormVM, selectedRoleIdsForUser);

            return View(userFormVM);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: User/Review
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Review(int userId)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserController][Review][HttpGet] => ")
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

            if (qmsUserVM.CanRetrieveUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(userId): {userId}");
            User userDB = _userService.RetrieveByUserId(userId);
            //User userDB = _userService.RetrieveByEmailAddress("testuser04@gsa.gov");
            Console.WriteLine(logSnippet + $"(userDB == null): {userDB == null}");

            UserFormViewModel userFormVM = _userFormUtil.MapToViewModel(userDB);
            Console.WriteLine(logSnippet + $"(userFormVM == null): {userFormVM == null}");

            userFormVM.Reactivatable        = true;
            userFormVM.Deactivatable        = false;
            userFormVM.Mutatatable          = false;
            userFormVM.AspAction            = "Review";
            userFormVM.SubmitButtonLabel    = "Review";
            userFormVM.CardHeader           = "Review QMS User:";
            //_userFormUtil.PopulateCheckboxRolesForCreateUser(userFormVM);

            // ORGANIZATIONS
            List<Organization> activeOrganizations = _organizationService.RetrieveActiveOrganizations();
            ViewBag.ActiveOrganizations = new SelectList(activeOrganizations, "OrgId", "OrgLabel", userDB.OrgId);

            // POTENTIAL MANAGERS
            if (userFormVM.ManagerId.HasValue && userFormVM.ManagerId.Value > 0)
            {
                List<User> usersInOrg = _userService.RetrieveUsersByOrganizationId(userFormVM.OrgId.Value);
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel", userFormVM.ManagerId);
            }
            else
            {
                List<User> usersInOrg = new List<User>();
                ViewBag.PotentialManagers = new SelectList(usersInOrg, "UserId", "DisplayLabel");
            }

            return View(userFormVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int userId)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserController][Deactivate][HttpPost] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanDeactivateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            User submitter = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            Console.WriteLine(logSnippet + $"(userId)...: '{userId}'");
            Console.WriteLine(logSnippet + $"(submitter == null): {submitter == null}");

            _userService.DeactivateUser(userId, submitter);
            
             UserListViewModel userListVM = new UserListViewModel
            {
                ShowAlert = true,
                AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS,
                AlertMessage = $"Deactivate QMS User #{userId} was successful!",
                UserAdminModule = UserAdminConstants.UserAdminModuleConstants.INACTIVE_USER,
                UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE,
                ActiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE,
                InactiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                InactiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
            };

            HttpContext.Session.SetObject(UserAdminConstants.USER_LIST_VIEW_MODEL, userListVM);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivate(int userId)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][UserController][Reactivate][HttpPost] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM == null): {qmsUserVM == null}");

            if (qmsUserVM.CanReactivateUser == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            User submitter = HttpContext.Session.GetObject<User>(MiscConstants.USER_SESSION_KEY);
            Console.WriteLine(logSnippet + $"(userId)...: '{userId}'");
            Console.WriteLine(logSnippet + $"(submitter == null): {submitter == null}");

            _userService.ReactivateUser(userId, submitter);
            
             UserListViewModel userListVM = new UserListViewModel
            {
                ShowAlert = true,
                AlertType = UserAdminConstants.AlertTypeConstants.SUCCESS,
                AlertMessage = $"Reactivate QMS User #{userId} was successful!",
                UserAdminModule = UserAdminConstants.UserAdminModuleConstants.ACTIVE_USER,
                UserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                UserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                ActiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.ACTIVE_NAVITEM_NAVLINK_VALUE,
                ActiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.ACTIVE_TABPANE_FADE_VALUE,
                InactiveUserNavItemNavLink = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE,
                InactiveUserTabPadFade = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE,
            };

            HttpContext.Session.SetObject(UserAdminConstants.USER_LIST_VIEW_MODEL, userListVM);
            return RedirectToAction(nameof(Index));
        }






    }
}