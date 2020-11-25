using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;
using QmsCore.UIModel;
using QmsCore.QmsException;
using QMS.Extensions;
using QMS.Constants;
using QMS.ViewModels;
using QMS.Utils;
using Qms_Data.UIModel;

namespace QMS.Controllers
{
    public class CorrectiveActionsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmployeeService _employeeService;
        private readonly ICorrectiveActionService _correctiveActionService;

        public CorrectiveActionsController(IUserService usrSvc, IEmployeeService empSvc, ICorrectiveActionService caSvc)
        {
            _userService = usrSvc;
            _employeeService = empSvc;
            _correctiveActionService = caSvc;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: CorrectiveAction/Index
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
         public IActionResult Index(string useCase, string sortOrder)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][CorrectiveActionsController][HttpGet][Index] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(useCase)..: '{useCase}'");
            Console.WriteLine(logSnippet + $"(sortOrder): '{sortOrder}'");

            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);

            List<CorrectiveActionListItem> svcList = null;
            string pageTitle = String.Empty;

            switch(useCase)
            {
                case "VCAFU":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_CORRECTIVE_ACTIONS_FOR_USER);
                    svcList = _correctiveActionService.RetrieveAllForUser(qmsUser);
                    pageTitle = "My Corrective Actions";
                break;
                case "VCAFO":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_CORRECTIVE_ACTIONS_FOR_ORG);
                    svcList = _correctiveActionService.RetrieveAllForOrganization(qmsUser);
                    pageTitle = "My Center's Actions";
                break;
                case "VCAFP":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_CORRECTIVE_ACTIONS_FOR_POID);
                    svcList = _correctiveActionService.RetrieveAllByEmployeePOID(qmsUser);
                    pageTitle = "Employees We Service";
                break;
                case "VCAALL":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_CORRECTIVE_ACTIONS);
                    svcList = _correctiveActionService.RetrieveAll();
                    pageTitle = "All Corrective Actions";
                break;
                case "VACAFU":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_USER);
                    svcList = _correctiveActionService.RetrieveAllForUserArchive(qmsUser);
                    pageTitle = "My Archived Actions";
                break;
                case "VACAFO":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_ORG);
                    svcList = _correctiveActionService.RetrieveAllForOrganizationArchive(qmsUser);
                    pageTitle = "My Center's Archived Actions";
                break;
                case "VACAFP":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ARCHIVED_CORRECTIVE_ACTIONS_FOR_POID);
                    svcList = _correctiveActionService.RetrieveAllByEmployeePOIDArchive(qmsUser);
                    pageTitle = "Employees We Service Archive";
                break;
                case "VACAALL":
                    this.doPermissionCheck(qmsUser, PermissionCodeConstants.VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS);
                    svcList = _correctiveActionService.RetrieveAllArchive();
                    pageTitle = "All Archived Corrective Actions";
                break;
                default:
                    return RedirectToAction("UnauthorizedAccess", "Home");
            }

            ViewData[CorrectiveActionsConstants.CA_INDEX_PAGE_TITLE] = pageTitle;

            Console.WriteLine(logSnippet + $"(svcList == null): '{svcList == null}'");
            if ( svcList != null)
            {
                 Console.WriteLine(logSnippet + $"(svcList.Count()): '{svcList.Count()}'");
            }

            ///////////////////////////////////////////////////////////////
            // PAGE TITLE
            ///////////////////////////////////////////////////////////////
            ViewData["CA_INDEX_PAGE_TITLE"] = pageTitle;

            ///////////////////////////////////////////////////////////////
            // USE CASE
            ///////////////////////////////////////////////////////////////
            ViewData["UseCase"] = useCase;

            IOrderedEnumerable<CorrectiveActionListItem> orderedSvcList = svcList.OrderBy(s => s.PriorityIndex);

            ///////////////////////////////////////////////////////////////
            // SORT ORDER
            ///////////////////////////////////////////////////////////////
            Console.WriteLine(logSnippet + $"(sortOrder): '{sortOrder}'");

            if (String.IsNullOrEmpty(sortOrder) )
            {
                ViewData["IdSortParam"]             = "id_desc";
                ViewData["emplIdSortParam"]         = "empl_id_desc";
                ViewData["emplNameSortParam"]       = "empl_name_desc";
                ViewData["requestTypeSortParam"]    = "request_type_desc";
                ViewData["noaSortParam"]            = "noa_desc";
                ViewData["orgSortParam"]            = "org_desc";
                ViewData["personSortParam"]         = "person_desc"; 
                ViewData["statusSortParam"]         = "status_desc";
                ViewData["prioritySortParam"]       = "priority_desc";
                ViewData["submittedBySortParam"]    = "submitted_by_desc";              
                ViewData["dateSubmittedSortParam"]  = "date_submitted_desc";
                ViewData["daysOldSortParam"]        = "days_old_desc";
            }
            else
            {
                switch(sortOrder)
                {
                    case "id_desc":
                        ViewData["IdSortParam"]  = "id_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.Id);
                        break;
                    case "id_asc":
                        ViewData["IdSortParam"]  = "id_desc";
                        orderedSvcList = svcList.OrderBy(s => s.Id);
                        break;
                    case "empl_id_desc":
                        ViewData["emplIdSortParam"]  = "empl_id_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.EmplId);
                        break;
                    case "empl_id_asc":
                        ViewData["emplIdSortParam"]  = "empl_id_desc";
                        orderedSvcList = svcList.OrderBy(s => s.EmplId);
                        break;
                    case "empl_name_desc":
                        ViewData["emplNameSortParam"]  = "empl_name_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.EmployeeName);
                        break;
                    case "empl_name_asc":
                        ViewData["emplNameSortParam"]  = "empl_name_desc";
                        orderedSvcList = svcList.OrderBy(s => s.EmployeeName);
                        break;
                    case "request_type_desc":
                        ViewData["requestTypeSortParam"]  = "request_type_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.RequestType);
                        break;
                    case "request_type_asc":
                        ViewData["requestTypeSortParam"]  = "request_type_desc";
                        orderedSvcList = svcList.OrderBy(s => s.RequestType);
                        break; 
                    case "noa_desc":
                        ViewData["noaSortParam"]  = "noa_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.NatureOfAction);
                        break;
                    case "noa_asc":
                        ViewData["noaSortParam"]  = "noa_desc";
                        orderedSvcList = svcList.OrderBy(s => s.NatureOfAction);
                        break;
                    case "org_desc":
                        ViewData["orgSortParam"]  = "org_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.OrgAssigned);
                        break;
                    case "org_asc":
                        ViewData["orgSortParam"]  = "org_desc";
                        orderedSvcList = svcList.OrderBy(s => s.OrgAssigned);
                        break;
                    case "person_desc":
                        ViewData["personSortParam"]  = "person_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.PersonAssigned);
                        break;
                    case "person_asc":
                        ViewData["personSortParam"]  = "person_desc";
                        orderedSvcList = svcList.OrderBy(s => s.PersonAssigned);
                        break;
                    case "status_desc":
                        ViewData["statusSortParam"]  = "status_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.Status);
                        break;
                    case "status_asc":
                        ViewData["statusSortParam"]  = "status_desc";
                        orderedSvcList = svcList.OrderBy(s => s.Status);
                        break;
                    case "priority_desc":
                        ViewData["prioritySortParam"]  = "priority_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.PriorityIndex);
                        break;
                    case "priority_asc":
                        ViewData["prioritySortParam"]  = "priority_desc";
                        orderedSvcList = svcList.OrderBy(s => s.PriorityIndex);
                        break;
                    case "submitted_by_desc":
                        ViewData["submittedBySortParam"]  = "submitted_by_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.SubmittedBy);
                        break;
                    case "submitted_by_asc":
                        ViewData["submittedBySortParam"]  = "submitted_by_desc";
                        orderedSvcList = svcList.OrderBy(s => s.SubmittedBy);
                        break; 
                    case "date_submitted_desc":
                        ViewData["dateSubmittedSortParam"]  = "date_submitted_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.DateSubmitted);
                        break;
                    case "date_submitted_asc":
                        ViewData["dateSubmittedSortParam"]  = "date_submitted_desc";
                        orderedSvcList = svcList.OrderBy(s => s.DateSubmitted);
                        break;   
                    case "days_old_desc":
                        ViewData["daysOldSortParam"]  = "days_old_asc";
                        orderedSvcList = svcList.OrderByDescending(s => s.DaysOld);
                        break;
                    case "days_old_asc":
                        ViewData["daysOldSortParam"]  = "days_old_desc";
                        orderedSvcList = svcList.OrderBy(s => s.DaysOld);
                        break;
                    default:
                        orderedSvcList = svcList.OrderBy(s => s.DateSubmitted);
                        break;
                }
            }
            
            return View(orderedSvcList);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: CorrectiveActionsController/Create
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        // [Authorize(Roles = "SC_SPECIALIST,SC_REVIEWER,PPRB_SPECIALIST,PPRB_REVIEWER")]
        public IActionResult Create()
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][CorrectiveActionsController][HttpGet][Create] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if ( qmsUserVM.CanCreateCorrectiveAction == false )
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            return View();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // POST: CorrectiveAction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "SC_SPECIALIST,SC_REVIEWER,PPRB_SPECIALIST,PPRB_REVIEWER")]
        public IActionResult Create
        (
            [Bind("EmployeeSearchResult, NatureOfAction,EffectiveDateOfPar,IsPaymentMismatch,ErrorTypeIds,ActionRequestTypeId,StatusTypeId,Details")] 
                CorrectiveActionFormViewModel correctiveActionVM, string[] selectedErrorTypes
        )
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                 return RedirectToAction("Warning", "Home");
            } 

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CorrectiveActionsController][HttpPost][Create] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if ( qmsUserVM.CanCreateCorrectiveAction == false )
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Error Categories Validation
            ///////////////////////////////////////////////////////////////////////////////////
            if ( selectedErrorTypes == null || selectedErrorTypes.Count() == 0 )
            {
                Console.WriteLine(logSnippet + "No error categories/types selected. Adding a model error.");
                ModelState.AddModelError(string.Empty, "Please select at least one error category.");
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Employee Search
            ///////////////////////////////////////////////////////////////////////////////////
            string emplIdStr = CorrectiveActionUtil.ExtractEmplId(correctiveActionVM.EmployeeSearchResult);
            long emplIdLong;
            if ( String.IsNullOrEmpty(emplIdStr) == false 
                    && long.TryParse(emplIdStr, out emplIdLong) == true)
            {
                try
                {
                    Employee employee = _employeeService.RetrieveById(emplIdStr);
                }
                catch(EmployeeNotFoundException enfe)
                {
                    Console.WriteLine(logSnippet + "EmployeeNotFoundException encountered:");
                    Console.WriteLine(enfe.Message);
                    ModelState.AddModelError(string.Empty, $"Employee not found with Employee ID of '{emplIdStr}'");
                }  
            }     

            Console.WriteLine(logSnippet + $"(ModelState.IsValid): {ModelState.IsValid}");

            if (ModelState.IsValid)
            {
                CorrectiveAction correctiveActionModel = CorrectiveActionUtil.MapToUIModelOnCreate(correctiveActionVM, selectedErrorTypes);                 
                User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);

                Console.WriteLine(logSnippet + "CorrectiveAction (BEFORE SAVE) => BEGIN");
                Console.WriteLine(correctiveActionModel.ToJson());
                Console.WriteLine(logSnippet + "END <= CorrectiveAction (BEFORE SAVE)");

                int entityId = _correctiveActionService.Save(correctiveActionModel, qmsUser);  

                Console.WriteLine(logSnippet 
                    + $"Newly created CorrectiveAction #{entityId} has been saved to the database, redirecting in Home Index page");

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // PREPARE SUCCESS MESSAGE
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                string msgKey = Guid.NewGuid().ToString();
                string msgVal = $"Corrective Action with an ID of {entityId} has been created for {correctiveActionVM.EmployeeSearchResult}";
                HttpContext.Session.SetObject(msgKey, msgVal);

                return RedirectToAction("Index", "Home", new { @mk = msgKey } );
            }
            else
            {
                Console.WriteLine(logSnippet + "ModelState IS NOT VALID, returning User to Create Corrective Actions page");
                if ( selectedErrorTypes != null || selectedErrorTypes.Count() > 0 )
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    // The HttpContext.Items collection is used to store data while processing a single request.
                    // The collection's contents are discarded after a request is processed.
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    HttpContext.Items[CorrectiveActionsConstants.SELECTED_ERROR_TYPES_KEY] = selectedErrorTypes;
                }
                
                return View();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // GET: CorrectiveActionsController/Edit
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        // [Authorize(Roles = "SC_SPECIALIST,SC_REVIEWER,PPRB_SPECIALIST,PPRB_REVIEWER,PPRM_SPECIALIST,PPRM_REVIEWER")]
        public IActionResult Edit(int? id, string useCase, string mk = null)
        {           
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                 return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CorrectiveActionsController][HttpGet][Edit] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if ( qmsUserVM.CanEditCorrectiveAction == false && qmsUserVM.CanViewAllCorrectiveActions == false)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            if (id == null || id.HasValue == false)
            {
                Console.WriteLine(logSnippet + $"ID that was passed-in is null or has no value. Cannot continue. (id: '{id}')");
                return NotFound();
            }

            Console.WriteLine(logSnippet + $"(CorectiveActionId): '{id}'");
            Console.WriteLine(logSnippet + $"(useCase)..........: '{useCase}'");
            
            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);
            CorrectiveAction entity = _correctiveActionService.RetrieveById(id.Value, qmsUser);

            Console.WriteLine(logSnippet + "(CorrectiveAction.Status == null): " + (entity.Status == null));          

            Console.WriteLine(logSnippet + "CorrectiveAction.ToJson() => BEGIN");
            Console.WriteLine(entity.ToJson());
            Console.WriteLine(logSnippet+ "END <= CorrectiveAction.ToJson()");

            HttpContext.Items[CorrectiveActionsConstants.NOA_CODE_KEY] = entity.NOACode;
            HttpContext.Items[CorrectiveActionsConstants.CURRENT_STATUS_ID_KEY] = entity.StatusId;

            string[] selectedErrorTypeIds = CorrectiveActionUtil.ExtractErrorTypeIds(entity);
            HttpContext.Items[CorrectiveActionsConstants.SELECTED_ERROR_TYPES_KEY] = selectedErrorTypeIds;

            Console.WriteLine(logSnippet + $"(qmsUserVM.CanAssignTasks).....: {qmsUserVM.CanAssignTasks}");
            Console.WriteLine(logSnippet + $"(CorrectiveAction.IsAssignable): {entity.IsAssignable}");

            HttpContext.Items[CorrectiveActionsConstants.IS_ASSIGNABLE_KEY] = false;
            HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY] = null; 
            
            if ( qmsUserVM.CanAssignTasks && entity.IsAssignable )
            {
                HttpContext.Items[CorrectiveActionsConstants.IS_ASSIGNABLE_KEY] = true;
                if (entity.AssignedByUserId.HasValue)
                {
                    HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY] = entity.AssignedToUserId.ToString();
                }
                else
                {
                    HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY] = null;
                }
            }     

            if ( HttpContext.Session.GetObject<bool>(CorrectiveActionsConstants.NEWLY_CREATED_COMMENT_KEY) == true)
            {
                ViewData[CorrectiveActionsConstants.NEWLY_CREATED_COMMENT_KEY] = HttpContext.Session.GetObject<bool>(CorrectiveActionsConstants.NEWLY_CREATED_COMMENT_KEY);
                HttpContext.Session.SetObject(CorrectiveActionsConstants.NEWLY_CREATED_COMMENT_KEY, false);
            }

            ViewData[CorrectiveActionsConstants.CA_EDIT_MESSAGE_KEY] = null;
            if (String.IsNullOrEmpty(mk) == false && String.IsNullOrWhiteSpace(mk) == false)
            {
                if (HttpContext.Session.GetObject<string>(mk) != null)
                {
                    ViewData[CorrectiveActionsConstants.CA_EDIT_MESSAGE_KEY] = HttpContext.Session.GetObject<string>(mk);
                    HttpContext.Session.SetObject(mk, null);
                }
            }

            List<ModuleMenuItem> moduleMenuItems = HttpContext.Session.GetObject<List<ModuleMenuItem>>(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);
    
            return View(CorrectiveActionUtil.MapToViewModel(entity, qmsUserVM.UserId, useCase, moduleMenuItems, qmsUserVM.CanAssignTasks));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // POST: CorrectiveAction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "SC_SPECIALIST,SC_REVIEWER,PPRB_SPECIALIST,PPRB_REVIEWER,PPRM_SPECIALIST,PPRM_REVIEWER")]
        public IActionResult Edit
        (
            [Bind("EmployeeSearchResult, NatureOfAction,EffectiveDateOfPar,IsPaymentMismatch,ErrorTypeIds,ActionRequestTypeId,StatusTypeId,CurrentStatusId,CorrectiveActionId,AssignedToUserId,RowVersion,Details")] 
                CorrectiveActionFormViewModel correctiveActionVM, string[] selectedErrorTypes
        )
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                  return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CorrectiveActionsController][HttpPost][Edit] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if ( qmsUserVM.CanEditCorrectiveAction == false )
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(correctiveActionVM.AssignedToUserId): '{correctiveActionVM.AssignedToUserId}'");
            Console.WriteLine(logSnippet + $"(correctiveActionVM.CurrentStatusId).: '{correctiveActionVM.CurrentStatusId}'");

            ///////////////////////////////////////////////////////////////////////////////////
            // Error Categories Validation
            ///////////////////////////////////////////////////////////////////////////////////
            if ( selectedErrorTypes == null || selectedErrorTypes.Count() == 0 )
            {
                Console.WriteLine(logSnippet + "No error categories/types selected. Adding a model error.");
                ModelState.AddModelError(string.Empty, "Please select at least one error category.");
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Employee Search
            ///////////////////////////////////////////////////////////////////////////////////
            string emplIdStr = CorrectiveActionUtil.ExtractEmplId(correctiveActionVM.EmployeeSearchResult);
            long emplIdLong;
            if ( String.IsNullOrEmpty(emplIdStr) == false 
                && long.TryParse(emplIdStr, out emplIdLong) == true)
            {
                try
                {
                    Employee employee = _employeeService.RetrieveById(emplIdStr);
                }
                catch(EmployeeNotFoundException enfe)
                {
                    Console.WriteLine(logSnippet + "EmployeeNotFoundException encountered:");
                    Console.WriteLine(enfe.Message);
                    ModelState.AddModelError(string.Empty, $"Employee not found with Employee ID of '{emplIdStr}'");
                }  
            }       

            User qmsUser = _userService.RetrieveByEmailAddress(qmsUserVM.EmailAddress);              
            CorrectiveAction oldCorrectiveAction = _correctiveActionService.RetrieveById(correctiveActionVM.CorrectiveActionId, qmsUser);

            if ( correctiveActionVM.CorrectiveActionId != oldCorrectiveAction.Id)
            {
                StringBuilder sb = new StringBuilder(logSnippet);
                sb.Append("Corrective Action ID discrepancy detected: ");
                sb.Append("UIModel ID: '");
                sb.Append(oldCorrectiveAction.Id);
                sb.Append("'; ViewModel ID: '");
                sb.Append(correctiveActionVM.CorrectiveActionId);
                sb.Append("'.");
                Console.WriteLine(sb.ToString());
            }

            if ( correctiveActionVM.RowVersion != oldCorrectiveAction.RowVersion)
            {
                StringBuilder sb = new StringBuilder(logSnippet);
                sb.Append("Row version discrepancy detected for Corrective Action (ID: '");
                sb.Append(oldCorrectiveAction.Id);
                sb.Append("'). UIModel Row Version: '");
                sb.Append(oldCorrectiveAction.RowVersion);
                sb.Append("'; ViewModel Row Version: '");
                sb.Append(correctiveActionVM.RowVersion);
                sb.Append("'.");
                Console.WriteLine(sb.ToString());
            }

            Console.WriteLine(logSnippet + "(ModelState.IsValid): " + (ModelState.IsValid));

            if (ModelState.IsValid)
            {
                CorrectiveAction updatedCorrectiveAction = CorrectiveActionUtil.MapToUIModelOnUpdate(correctiveActionVM, selectedErrorTypes, oldCorrectiveAction); 

                Console.WriteLine(logSnippet + "Updated CorrectiveAction (BEFORE SAVE) => BEGIN");
                Console.WriteLine(updatedCorrectiveAction.ToJson());
                Console.WriteLine("END <= Updated CorrectiveAction (BEFORE SAVE)");                      
                Console.WriteLine(logSnippet + $"(updatedCorrectiveAction.AssignedToUserId)(BEFORE SAVE): '{updatedCorrectiveAction.AssignedToUserId}'");

                int entityId = 0;
                try
                {
                    entityId = _correctiveActionService.Save(updatedCorrectiveAction, qmsUser); 
                }
                catch(LockingException lockExc)
                {
                    Console.WriteLine(logSnippet + "(BEGIN CorrectiveActionLockingException:");
                    Console.WriteLine(lockExc.Message);
                    Console.WriteLine(logSnippet + "(:END CorrectiveActionLockingException");
                }
                 
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // WRITE SUCCESS MESSAGE TO LOG
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                Console.WriteLine(logSnippet + "Record saved to DB, redirecting in CorrectiveAction Index page");
                Console.WriteLine(logSnippet + $"Updated CorrectiveAction #{updatedCorrectiveAction.Id} has been saved to the database, redirecting in Home Index page");

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // PREPARE SUCCESS MESSAGE FOR USER
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                string msgKey = Guid.NewGuid().ToString();
                string msgVal = $"Corrective Action with an ID of {entityId} has been updated for {correctiveActionVM.EmployeeSearchResult}";
                HttpContext.Session.SetObject(msgKey, msgVal);

                return RedirectToAction("Index", "Home", new { @mk = msgKey } );
            }
            else
            {
                Console.WriteLine(logSnippet + "ModelState IS NOT VALID, returning User to Edit Corrective Actions page");
                if ( selectedErrorTypes != null || selectedErrorTypes.Count() > 0 )
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    // The HttpContext.Items collection is used to store data while processing a single request.
                    // The collection's contents are discarded after a request is processed.
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    HttpContext.Items[CorrectiveActionsConstants.SELECTED_ERROR_TYPES_KEY] = selectedErrorTypes;
                    HttpContext.Items[CorrectiveActionsConstants.CURRENT_STATUS_ID_KEY] = correctiveActionVM.CurrentStatusId;
                }

                if ( qmsUserVM.CanAssignTasks && oldCorrectiveAction.IsAssignable )
                {
                    HttpContext.Items[CorrectiveActionsConstants.IS_ASSIGNABLE_KEY] = true;
                    if (oldCorrectiveAction.AssignedByUserId.HasValue)
                    {
                        HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY] = oldCorrectiveAction.AssignedToUserId.ToString();
                    }
                    else
                    {
                        HttpContext.Items[CorrectiveActionsConstants.CURRENT_ASSIGNED_TO_USER_ID_KEY] = null;
                    }
                }   

                correctiveActionVM.CorrectiveActionId               = oldCorrectiveAction.Id;
                correctiveActionVM.CorrectiveActionIdForAddComment  = oldCorrectiveAction.Id;
                correctiveActionVM.Details                          = oldCorrectiveAction.Details;
                correctiveActionVM.Comments                         = oldCorrectiveAction.Comments;
                correctiveActionVM.Histories                         = oldCorrectiveAction.Histories;

                correctiveActionVM.StatusLabel          = oldCorrectiveAction.Status.StatusLabel;
                correctiveActionVM.DateSubmitted        = oldCorrectiveAction.CreatedAt.ToString("MMMM dd, yyyy");
                correctiveActionVM.CreatedByUserName    = oldCorrectiveAction.CreatedByUser.DisplayName;
                correctiveActionVM.CreatedByOrgLabel    = oldCorrectiveAction.CreatedByOrg.OrgLabel;

                correctiveActionVM.IsReadOnly           = oldCorrectiveAction.IsReadOnly;

                return View(correctiveActionVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comment
        (
            [Bind("CorrectiveActionIdForAddComment,Comment")] 
                CorrectiveActionFormViewModel correctiveActionVM
        )
        {
            if ( HttpContext == null 
                    || HttpContext.Session == null 
                    || HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY) == null)
            {
                 return RedirectToAction("Warning", "Home");
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][CorrectiveActionsController][HttpPost][Comment] => ")
                    .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            if ( qmsUserVM.CanCommentOnTask == false )
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            Console.WriteLine(logSnippet + $"(CorrectiveAction.Id): '{correctiveActionVM.CorrectiveActionIdForAddComment}'");

            _correctiveActionService.SaveComment(correctiveActionVM.Comment, correctiveActionVM.CorrectiveActionIdForAddComment, qmsUserVM.UserId);

            HttpContext.Session.SetObject(CorrectiveActionsConstants.NEWLY_CREATED_COMMENT_KEY, true);

            Console.WriteLine(logSnippet + "Redirecting to [CorrectiveActionsController][HttpGet][Edit]");

            return RedirectToAction(nameof(Edit), new{ @id = correctiveActionVM.CorrectiveActionIdForAddComment});
        }

        private IActionResult doPermissionCheck(User qmsUser, string permissionCode)
        {
            if ( UserUtil.UserHasThisPermission(qmsUser, permissionCode) )
            {
                return null;
            }
            return RedirectToAction("UnauthorizedAccess", "Home");
        }
    }
}