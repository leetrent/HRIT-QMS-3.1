using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QmsCore.Services;
using QmsCore.Model;
using QmsCore.UIModel;
using QMS.Extensions;
using QMS.ViewModels;
using QMS.Constants;
using QMS.Utils;

namespace QMS.Controllers
{
    public class DataErrorsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmployeeService _employeeService;
        private readonly IDataErrorService _dataErrorService;
        private readonly IReferenceService _referenceService;

        public DataErrorsController(IUserService usrSvc, IEmployeeService empSvc, IDataErrorService deSvc, IReferenceService refSvc)
        {
            _userService = usrSvc;
            _employeeService = empSvc;
            _dataErrorService = deSvc;
            _referenceService = refSvc;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: DataErrors/Index
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Index(string useCase, string sortOrder)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][DataErrorsController][HttpGet][Index] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(useCase)..: '{useCase}'");
            Console.WriteLine(logSnippet + $"(sortOrder): '{sortOrder}'");

            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);

            List<DataErrorListItem> svcList = null;
            string pageTitle = String.Empty;

            if ( useCase.Equals(UseCaseConstants.VIEW_ALL_EHRI_ERRORS) )
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_EHRI_ERRORS);
                svcList = _dataErrorService.RetrieveAll();
                pageTitle = PageTitleConstants.VIEW_ALL_EHRI_ERRORS;
            }
            else if ( useCase.Equals(UseCaseConstants.VIEW_ALL_ARCHIVED_EHRI_ERRORS) )
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_ARCHIVED_EHRI_ERRORS);
                svcList = _dataErrorService.RetrieveAllArchive();
                pageTitle = PageTitleConstants.VIEW_ALL_ARCHIVED_EHRI_ERRORS;
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_EHRI_ERRORS_FOR_USER))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_EHRI_ERRORS_FOR_USER);
                svcList = _dataErrorService.RetrieveAllForUser(qmsUser); 
                pageTitle = PageTitleConstants.VIEW_EHRI_ERRORS_FOR_USER;
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_EHRI_ERRORS_FOR_ORG))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_EHRI_ERRORS_FOR_ORG);
                svcList = _dataErrorService.RetrieveAllByOrganization(qmsUser);
                pageTitle = PageTitleConstants.VIEW_EHRI_ERRORS_FOR_ORG;
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER);
                svcList = _dataErrorService.RetrieveAllForUserArchive(qmsUser);
                pageTitle = PageTitleConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER;
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG);
                svcList = _dataErrorService.RetrieveAllByOrganizationArchive(qmsUser);
                pageTitle = PageTitleConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG;
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            ///////////////////////////////////////////////////////////////
            // DEBUG
            ///////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(svcList == null): {svcList == null}");
            Console.WriteLine(logSnippet + $"(svcList.Count)..: {svcList.Count}");
            //foreach (DataErrorListItem item in svcList)
            //{
            //    Console.WriteLine(logSnippet + $"(EhriErrorId): '{item.EhriErrorId}' / (Category): '{item.Category}'");
            //}

            ///////////////////////////////////////////////////////////////
            // PAGE TITLE
            ///////////////////////////////////////////////////////////////
            ViewData[DataErrorConstants.DATA_ERRORS_INDEX_PAGE_TITLE] = pageTitle;

            ///////////////////////////////////////////////////////////////
            // USE CASE
            ///////////////////////////////////////////////////////////////
            ViewData["UseCase"] = useCase;
            foreach (DataErrorListItem item in svcList)
            {
                item.UseCase = useCase;
            }


            IOrderedEnumerable<DataErrorListItem> orderedSvcList = svcList.OrderBy(s => s.PriorityIndex);
 
            ///////////////////////////////////////////////////////////////
            // SORT ORDER
            ///////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(sortOrder): '{sortOrder}'");
            if (String.IsNullOrEmpty(sortOrder))
            {
                ViewData["IdSortParam"]             = "id_desc";
                ViewData["emplIdSortParam"]         = "empl_id_desc";
                ViewData["emplNameSortParam"]       = "empl_name_desc";
                ViewData["officeSymbolSortParam"]   = "office_symbol_desc";
                ViewData["errorCodeSortParam"]      = "error_code_desc";
                ViewData["dataElementSortParam"]    = "data_element_desc";
                ViewData["statusSortParam"]         = "status_desc";
                ViewData["assignedToSortParam"]     = "assigned_to_desc";
                ViewData["dateCreatedSortParam"]    = "date_created_desc";
                ViewData["priorityIndexSortParam"]  = "priority_index_desc";
                ViewData["daysOpenSortParam"]       = "days_open_desc";
                ViewData["caIdSortParam"]           = "ca_id_desc";
            }
            else
            {
                switch (sortOrder)
                {
                    case "id_desc":
                        ViewData["IdSortParam"] = "id_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.EhriErrorId);
                        break;
                    case "id_asc":
                        ViewData["IdSortParam"] = "id_desc";
                        orderedSvcList = svcList.OrderBy(s => s.EhriErrorId);
                        break;
                    case "empl_id_desc":
                        ViewData["emplIdSortParam"] = "empl_id_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.EmplId);
                        break;
                    case "empl_id_asc":
                        ViewData["emplIdSortParam"] = "empl_id_desc";
                        orderedSvcList = svcList.OrderBy(s => s.EmplId);
                        break;
                    case "empl_name_desc":
                        ViewData["emplNameSortParam"] = "empl_name_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.EmployeeName);
                        break;
                    case "empl_name_asc":
                        ViewData["emplNameSortParam"] = "empl_name_desc";
                        orderedSvcList = svcList.OrderBy(s => s.EmployeeName);
                        break;
                    case "office_symbol_desc":
                        ViewData["officeSymbolSortParam"] = "office_symbol_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.OfficeSymbol);
                        break;
                    case "office_symbol_asc":
                        ViewData["officeSymbolSortParam"] = "office_symbol_desc";
                        orderedSvcList = svcList.OrderBy(s => s.OfficeSymbol);
                        break;
                    case "error_code_desc":
                        ViewData["errorCodeSortParam"] = "error_code_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.ErrorCode);
                        break;
                    case "error_code_asc":
                        ViewData["errorCodeSortParam"] = "error_code_desc";
                        orderedSvcList = svcList.OrderBy(s => s.ErrorCode);
                        break;
                    case "data_element_desc":
                        ViewData["dataElementSortParam"] = "data_element_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.DataElement);
                        break;
                    case "data_element_asc":
                        ViewData["dataElementSortParam"] = "data_element_desc";
                        orderedSvcList = svcList.OrderBy(s => s.DataElement);
                        break;
                    case "status_desc":
                        ViewData["statusSortParam"] = "status_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.Status);
                        break;
                    case "status_asc":
                        ViewData["statusSortParam"] = "status_desc";
                        orderedSvcList = svcList.OrderBy(s => s.Status);
                        break;
                    case "assigned_to_desc":
                        ViewData["assignedToSortParam"] = "assigned_to_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.AssignedTo);
                        break;
                    case "assigned_to_asc":
                        ViewData["assignedToSortParam"] = "assigned_to_desc";
                        orderedSvcList = svcList.OrderBy(s => s.AssignedTo);
                        break;
                    case "date_created_desc":
                        ViewData["dateCreatedSortParam"] = "date_created_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.DateCreated);
                        break;
                    case "date_created_asc":
                        ViewData["dateCreatedSortParam"] = "date_created_desc";
                        orderedSvcList = svcList.OrderBy(s => s.DateCreated);
                        break;
                    case "priority_index_desc":
                        ViewData["priorityIndexSortParam"] = "priority_index_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.PriorityIndex);
                        break;
                    case "priority_index_asc":
                        ViewData["priorityIndexSortParam"] = "priority_index_desc";
                        orderedSvcList = svcList.OrderBy(s => s.PriorityIndex);
                        break;
                    case "days_open_desc":
                        ViewData["daysOpenSortParam"] = "days_open_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.DaysOpen);
                        break;
                    case "days_open_asc":
                        ViewData["daysOpenSortParam"] = "days_open_desc";
                        orderedSvcList = svcList.OrderBy(s => s.DaysOpen);
                        break;
                    case "ca_id_desc":
                        ViewData["caIdSortParam"] = "ca_id_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.CorrectiveActionId);
                        break;
                    case "ca_id_asc":
                        ViewData["caIdSortParam"] = "ca_id_desc";
                        orderedSvcList = svcList.OrderBy(s => s.CorrectiveActionId);
                        break;
                    default:
                        orderedSvcList = svcList.OrderBy(s => s.PriorityIndex);
                        break;
                }
            }
            return View(orderedSvcList);

        }

        private IActionResult doPermissionCheck(User qmsUser, string permissionCode)
        {
            if (UserUtil.UserHasThisPermission(qmsUser, permissionCode))
            {
                return null;
            }
            return RedirectToAction("UnauthorizedAccess", "Home");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: CorrectiveActionsController/Edit
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Edit(int id, string useCase)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][DataErrorsController][HttpGet][Edit] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(id).....: '{id}'");
            Console.WriteLine(logSnippet + $"(useCase).: '{useCase}'");

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // PERMISSION CHECK
            //////////////////////////////////////////////////////////////////////////////////////////////////
            if (useCase.Equals(UseCaseConstants.VIEW_ALL_EHRI_ERRORS))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_EHRI_ERRORS);
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_ALL_ARCHIVED_EHRI_ERRORS))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_ARCHIVED_EHRI_ERRORS);
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_EHRI_ERRORS_FOR_USER))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_EHRI_ERRORS_FOR_USER);
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_EHRI_ERRORS_FOR_ORG))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_EHRI_ERRORS_FOR_ORG);
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_USER);
            }
            else if (useCase.Equals(UseCaseConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG))
            {
                this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_EHRI_ERRORS_FOR_ORG);
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // RETRIEVE DATA ERROR UI MODEL CLASS FROM DATA ERROR SERVICE
            //////////////////////////////////////////////////////////////////////////////////////////////////
            DataError svcDataError = _dataErrorService.RetrieveById(id, qmsUser);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // ACTION DROPDOWN
            //////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(svcDataError.StatusId.HasValue): '{svcDataError.StatusId.HasValue}'");
            Console.WriteLine(logSnippet + $"(svcDataError.StatusId): '{svcDataError.StatusId}'");
            List<Status> statusList = _referenceService.RetrieveAvailableActionsList(svcDataError.StatusId.Value, qmsUser.Organization, WorkItemTypeEnum.EHRI);
            ViewBag.StatusTypeItems = new SelectList(statusList, "StatusId", "StatusLabel");

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // ASSIGNEE DROPDOWN
            //////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(qmsUserVM.CanAssignTasks).: '{qmsUserVM.CanAssignTasks}'");
            Console.WriteLine(logSnippet + $"(svcDataError.IsAssignable): '{svcDataError.IsAssignable}'");

            if (qmsUserVM.CanAssignTasks && svcDataError.IsAssignable)
            {
                List<User> usersByOrgList = _userService.RetrieveUsersByOrganizationId(qmsUser.OrgId.Value);
                if (svcDataError.AssignedByUserId.HasValue)
                {
                    ViewBag.AssignedToUserItems = new SelectList(usersByOrgList, "UserId", "DisplayName", svcDataError.AssignedByUserId.Value);
                }
                else
                {
                    ViewBag.AssignedToUserItems = new SelectList(usersByOrgList, "UserId", "DisplayName");
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // NEED TO FIND THE CORRECT 'LIST' CONTROLLER (CA or DE) BASED ON THE USE CASE VALUE PASSED INTO THIS METHOD
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<ModuleMenuItem> moduleMenuItems = HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // MAP DATA ERROR UI MODEL CLASS TO DATA ERROR VIEW MODEL CLASS
            //////////////////////////////////////////////////////////////////////////////////////////////////
            DataErrorViewModel vmDataError = DataErrorUtil.MapToViewModel(qmsUserVM, svcDataError, useCase, moduleMenuItems);

            return View(vmDataError);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("DataErrorId,StatusId,Details,UseCase,EmployeeName,AssignedToUserId")] DataErrorViewModel vmDataError)
        {
            if (HttpContext == null
                    || HttpContext.Session == null
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][DataErrorsController][Edit][HttpPost] => ")
                                .ToString();
            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            Console.WriteLine(logSnippet + $"(vmDataError.DataErrorId).....: '{vmDataError.DataErrorId}'");
            Console.WriteLine(logSnippet + $"(vmDataError.StatusId)........: '{vmDataError.StatusId}'");
            Console.WriteLine(logSnippet + $"(vmDataError.Details).........: '{vmDataError.Details}'");
            Console.WriteLine(logSnippet + $"(vmDataError.UseCase).........: '{vmDataError.UseCase}'");
            Console.WriteLine(logSnippet + $"(vmDataError.EmployeeName)....: '{vmDataError.EmployeeName}'");
            Console.WriteLine(logSnippet + $"(vmDataError.AssignedToUserId): '{vmDataError.AssignedToUserId}'");
            Console.WriteLine(logSnippet + $"(ModelState.IsValid)..........: '{ModelState.IsValid}'");


            if (ModelState.IsValid)
            {
                int? correctiveActionId = _dataErrorService.Save(vmDataError.DataErrorId, vmDataError.Details, Int32.Parse(vmDataError.StatusId), qmsUser, vmDataError.AssignedToUserId);
                Console.WriteLine(logSnippet + $"(correctiveActionId.HasValue): '{correctiveActionId.HasValue}'");
                Console.WriteLine(logSnippet + $"(correctiveActionId).........: '{correctiveActionId}'");

                if (correctiveActionId.HasValue)
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // PREPARE SUCCESS MESSAGE FOR USER
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                    string msgKey = Guid.NewGuid().ToString();
                    string msgVal = $"You have successfully converted Data Error #{vmDataError.DataErrorId} for {vmDataError.EmployeeName} into the below Corrective Action #{correctiveActionId.Value}.";
                    HttpContext.Session.SetObject(msgKey, msgVal);

                    return RedirectToAction("Edit", "CorrectiveActions", new { @id = correctiveActionId.Value, @useCase = vmDataError.UseCase, @mk = msgKey });
                }
                else
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // PREPARE SUCCESS MESSAGE FOR USER
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                    string msgKey = Guid.NewGuid().ToString();
                    string msgVal = $"EHRI/Data Error with an ID of {vmDataError.DataErrorId} has been updated for {vmDataError.EmployeeName}";
                    HttpContext.Session.SetObject(msgKey, msgVal);

                    return RedirectToAction("Index", "Home", new { @mk = msgKey });
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // INVALID MODEL STATE PROCESSING
            //////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // RETRIEVE DATA ERROR UI MODEL CLASS FROM DATA ERROR SERVICE
            //////////////////////////////////////////////////////////////////////////////////////////////////
            DataError svcDataError = _dataErrorService.RetrieveById(vmDataError.DataErrorId, qmsUser);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // ACTION DROPDOWN
            //////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(svcDataError.StatusId.HasValue): '{svcDataError.StatusId.HasValue}'");
            Console.WriteLine(logSnippet + $"(svcDataError.StatusId): '{svcDataError.StatusId}'");
            List<Status> statusList = _referenceService.RetrieveAvailableActionsList(svcDataError.StatusId.Value, qmsUser.Organization, WorkItemTypeEnum.EHRI);
            ViewBag.StatusTypeItems = new SelectList(statusList, "StatusId", "StatusLabel");

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // NEED TO FIND THE CORRECT 'LIST' CONTROLLER (CA or DE) BASED ON THE USE CASE VALUE PASSED INTO THIS METHOD
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<ModuleMenuItem> moduleMenuItems = HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // MAP DATA ERROR UI MODEL CLASS TO DATA ERROR VIEW MODEL CLASS
            //////////////////////////////////////////////////////////////////////////////////////////////////
            DataErrorViewModel vmDataError2 = DataErrorUtil.MapToViewModel(qmsUserVM, svcDataError, vmDataError.UseCase, moduleMenuItems);

            return View(vmDataError2);
        }
    }
}