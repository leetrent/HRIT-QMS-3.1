using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Engine;
using QmsCore.Lib;
using QmsCore.QmsException;
using System.Text;
using Qms_Data.UIModel;

namespace QmsCore.Services
{
public class CorrectiveActionService : BaseService<QmsCorrectiveactionrequest>, ICorrectiveActionService
    {
        internal CorrectiveActionRepository correctiveActionRepository;
        internal ReferenceRepository referenceRepository;
        internal ReferenceService referenceService;
        internal int archiveDayCount = -30; //-30 

#region "Constructor's"

        public CorrectiveActionService()
        {
            correctiveActionRepository = new CorrectiveActionRepository();
            referenceRepository = new ReferenceRepository();
            referenceService = new ReferenceService();
        }

        public CorrectiveActionService(ICorrectiveActionRepository correctiveActionRepository)
        {
            referenceRepository = new ReferenceRepository();
            referenceService = new ReferenceService();
            this.correctiveActionRepository = (CorrectiveActionRepository)correctiveActionRepository;
        }

        public CorrectiveActionService(QMSContext context)
        {
            referenceRepository = new ReferenceRepository(context);
            referenceService = new ReferenceService(context);
            this.correctiveActionRepository = new CorrectiveActionRepository(context);
        }


#endregion

#region "Update methods"

        public void SaveComment(string message
                                      ,int correctiveActionId
                                      ,int authorId)
        {
            CorrectiveActionComment comment = new CorrectiveActionComment(message,correctiveActionId,authorId);
            QmsWorkitemcomment workItemComment = comment.WorkItemComment();
            this.correctiveActionRepository.context.Add(workItemComment);
            this.correctiveActionRepository.Save();
        }



        public int Save(CorrectiveAction newVersion, User submitter)
        {
            string history = string.Empty;
            if(newVersion.EmplId.Length > 8)
            {
                newVersion.EmplId = newVersion.EmplId.Trim();
                if(newVersion.EmplId.Length != 8)
                {
                    throw new Exception(string.Format("Invalid EmployeeId: {0}. Length doesn't equal 8.",newVersion.EmplId));
                }
            }
            if(newVersion.Id > 0)
            {
                CorrectiveAction oldVersion = RetrieveById(newVersion.Id);            
                if(oldVersion.RowVersion == newVersion.RowVersion)
                {
                    history = writeHistory(newVersion,oldVersion,submitter);
                    newVersion.RowVersion++;
                    CorrectiveActionRoutingEngine correctiveActionEngine = new CorrectiveActionRoutingEngine(this);
                    int retval = correctiveActionEngine.ExecuteUpdates(newVersion,submitter,history);
                    CorrectiveActionNotificationEngine correctiveActionNotificationEngine = new CorrectiveActionNotificationEngine(); 
                    correctiveActionNotificationEngine.Send(newVersion,correctiveActionEngine.NotificationEventType, submitter);            
                    return retval; 
                }
                else
                {
                    throw new LockingException(string.Format("This Corrective Action {0} was updated by another user. Old version #{1}, new version #{2}",newVersion.Id, newVersion.RowVersion, oldVersion.RowVersion));
                }
            }
            else  //new CA
            {
                CorrectiveActionRoutingEngine correctiveActionEngine = new CorrectiveActionRoutingEngine(this);
                int retval = correctiveActionEngine.ExecuteUpdates(newVersion,submitter,history);
                CorrectiveActionNotificationEngine correctiveActionNotificationEngine = new CorrectiveActionNotificationEngine(); 
                correctiveActionNotificationEngine.Send(newVersion,correctiveActionEngine.NotificationEventType, submitter);            
                return retval; 
            }

        }


#endregion

#region "Generate History"

    private string writeHistory(CorrectiveAction newVersion, CorrectiveAction oldVersion, User submitter)
    {
        StringBuilder sb = new StringBuilder();
        int changeCount = 0;
        string lineBreak = "<br/>";

        if(newVersion.EmplId != oldVersion.EmplId)
        {
            changeCount++;
            sb.AppendLine(string.Format(" - Changed EmplID from: {0} to {1}{2}",oldVersion.EmplId.ToString(),newVersion.EmplId.ToString(),lineBreak));            
        }
        
        if(newVersion.NOACode != oldVersion.NOACode)
        {
            changeCount++;
            sb.AppendLine(string.Format(" - Changed NOAC from: {0} to {1}{2}",oldVersion.NOACode,newVersion.NOACode,lineBreak));
        }

        if(newVersion.EffectiveDateOfPar != oldVersion.EffectiveDateOfPar)
        {
            changeCount++;
            sb.AppendLine(string.Format(" - Changed Effective Date of PAR from: {0} to {1}{2}",oldVersion.EffectiveDateOfPar.ToShortDateString(),newVersion.EffectiveDateOfPar.ToShortDateString(),lineBreak));            
        }

        if(newVersion.IsPaymentMismatch != oldVersion.IsPaymentMismatch)
        {
            changeCount++;
            sb.AppendLine(string.Format(" - Changed Payment Mismatch from: {0} to {1}{2}",oldVersion.IsPaymentMismatch,newVersion.IsPaymentMismatch,lineBreak));
        }

        if(newVersion.Details != oldVersion.Details)
        {
            changeCount++;
            sb.AppendLine(string.Format(" - Details changed from: {2} {0} to {2} {1} {2}",oldVersion.Details,newVersion.Details,lineBreak));            
        }


        if(newVersion.ActionRequestTypeId != oldVersion.ActionRequestTypeId)
        {
            changeCount++;
            var newItem = referenceRepository.RetrieveActionTypes().Where(a => a.Id == newVersion.ActionRequestTypeId).SingleOrDefault();
            sb.AppendLine(string.Format(" - Action Type has been updated to {0}{1}",newItem.Label,lineBreak));
        }

        List<int> oldErrorTypeIds = new List<int>();
        List<int> newErrorTypeIds = new List<int>();

        foreach(var existingItem in oldVersion.ErrorTypes)
        {
            oldErrorTypeIds.Add(existingItem.Id.Value);
        }        
        foreach(var newItem in newVersion.ErrorTypes)
        {
            newErrorTypeIds.Add(newItem.Id.Value);
        }

        foreach(var existingItem in oldErrorTypeIds)
        {
            if(!newErrorTypeIds.Contains(existingItem))
            {
                ErrorType errorType = referenceService.RetrieveErrorTypes().Where(e => e.Id == existingItem).SingleOrDefault();
                changeCount++;
                sb.AppendLine(string.Format(" - Removed Error Type: {0} {1}",errorType.Description,lineBreak));
            }
        }

        foreach(var newItem in newErrorTypeIds)
        {
            if(!oldErrorTypeIds.Contains(newItem))
            {
                ErrorType errorType = referenceService.RetrieveErrorTypes().Where(e => e.Id == newItem).SingleOrDefault();
                changeCount++;
                sb.AppendLine(string.Format(" - Added Error Type: {0} {1}",errorType.Description,lineBreak));             
            }
        }


        
        sb.AppendLine(string.Format("{0}{1} change(s) to Corrective Action made by {2}",lineBreak,changeCount,submitter.DisplayName));
        Console.WriteLine(sb.ToString());
        return sb.ToString();
    }

        #endregion

        #region "Retrieve Methods"

        private List<CorrectiveActionListItem> convertCorrectiveActionListItems(IQueryable<CorrectiveAction> items)
        {
            List<CorrectiveActionListItem> retval = new List<CorrectiveActionListItem>();
            foreach (var item in items)
            {
                retval.Add(item.CorrectiveActionListItem());
            }

            return retval;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="searcher"></param>
        /// <returns></returns>
        public CorrectiveAction RetrieveById(int Id, User searcher) 
        {
            var item = correctiveActionRepository.RetrieveById(Id);
            CorrectiveAction retval = new CorrectiveAction(item,searcher);
            retval.Comments = RetrieveComments(Id);
            retval.Histories = RetrieveHistory(Id);
            addSearchHistory(item, searcher);
            return retval;            
        }

        internal CorrectiveAction RetrieveById(int Id)
        {
            var item = correctiveActionRepository.RetrieveById(Id);
            CorrectiveAction retval = new CorrectiveAction(item);
//s            retval.Comments = retrieveComments(Id);
            return retval;            
        }

#endregion
        public List<CorrectiveActionComment> RetrieveComments(int correctiveActionId)
        {
            bool enableUserSecurityLoading = false;
            var items = correctiveActionRepository.context.QmsWorkitemcomment.AsNoTracking().Where(c => c.WorkItemId == correctiveActionId && c.WorkItemTypeCode == "CorrectiveActionRequest").Include(c => c.WorkItemTypeCodeNavigation).Include(c => c.Author).ThenInclude(u => u.Org);
            List<CorrectiveActionComment> comments = new List<CorrectiveActionComment>();
            foreach(var item in items)
            {
                comments.Add(new CorrectiveActionComment(item,enableUserSecurityLoading));
            }
            return comments;
        }

        public List<CorrectiveActionHistory> RetrieveHistory(int correctiveActionId)
        {
            var items = correctiveActionRepository.context.QmsWorkitemhistory.AsNoTracking().Where(c => c.WorkItemId == correctiveActionId && c.WorkItemTypeCode == "CorrectiveActionRequest").Include(c => c.PreviousAssignedByUser).Include(c => c.PreviousAssignedToOrg).Include(c => c.PreviousStatus).Include(c => c.PreviousAssignedtoUser).Include(c => c.ActionTakenByUser).Include(c => c.WorkItemTypeCodeNavigation); 
            List<CorrectiveActionHistory> histories = new List<CorrectiveActionHistory>();
            foreach(var item in items)
            {
                histories.Add(new CorrectiveActionHistory(item));
            }
            return histories;
        }

        public List<CorrectiveActionListItem> RetrieveAgingReportForUser(User user)
        {
            return convertCorrectiveActionListItems(retrieveAgingReportForUser(user));
        }

        private IQueryable<CorrectiveAction> retrieveAgingReportForUser(User user)
        {
            bool userIsAnSCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.SC_SPECIALIST);
            bool userIsPPRBSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_SPECIALIST);
            bool userIsAPPRMSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_SPECIALIST);
            bool userIsAnSCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.SC_REVIEWER);
            bool userIsPPRBReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_REVIEWER);
            bool userIsAPPRMReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_REVIEWER);
            bool userIsCorrectiveActionViewer = UserUtil.UserHasRole(user,ApplicationRoleType.CA_VIEWER);
            if(userIsAPPRMSpecialist)
            {
                return null;
            }
            else if(userIsPPRBSpecialist)
            {
                return null;
            }
            else if(userIsAnSCSpecialist)
            {
                return null;
             }
            else if(userIsAPPRMReviewer)
            {
                return null;
            }
            else if(userIsPPRBReviewer)
            {
                return null;
            }
            else if(userIsAnSCReviewer)
            {
                return null;
            }
            else if(userIsCorrectiveActionViewer)
            {
                return null;
            }
            else
            {
                throw new Exception("Role to View Aging report not found");
            }
        }


        public List<CorrectiveActionListItem> RetrieveAllForUser(User user)
        {
            bool userIsAnSCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.SC_SPECIALIST);
            bool userIsPPRBSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_SPECIALIST);
            bool userIsAPPRMSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_SPECIALIST);



            if (userIsAPPRMSpecialist)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToUser(user.UserId));
            }
            else if(userIsPPRBSpecialist)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToOrCreatedByUserId(user.UserId));
            }
            else if(userIsAnSCSpecialist)
            {
                return convertCorrectiveActionListItems(RetrieveAllByCreatedBy(user.UserId));
            }
            else
            {
                throw new Exception("Permission to View By User not found");
            }
        }




        /// <summary>
        /// For SC, BRC or PPRM Specialist only
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<CorrectiveActionListItem> RetrieveAllForUserArchive(User user)
        {
            bool userIsAnSCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.SC_SPECIALIST);
            bool userIsPPRBSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_SPECIALIST);
            bool userIsAPPRMSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_SPECIALIST);


            if(userIsAPPRMSpecialist)
            {
                return convertCorrectiveActionListItems(retrieveAllArchive());
            }
            else if(userIsPPRBSpecialist)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToOrCreatedByUserIdArchive(user.UserId));
            }
            else if(userIsAnSCSpecialist)
            {
                return convertCorrectiveActionListItems(RetrieveAllByCreatedByArchive(user.UserId));
            }
            else
            {
                throw new Exception("Permission to View By User not found");
            }
        }


        /// <summary>
        /// For SC, BRC or PPRM Reviewer Only
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<CorrectiveActionListItem> RetrieveAllForOrganization(User user)
        {
            bool userIsAnSCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.SC_REVIEWER);
            bool userIsPPRBReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_REVIEWER);
            bool userIsAPPRMReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_REVIEWER);


            if(userIsAPPRMReviewer)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToOrCreatedByOrg(user.OrgId.Value));
            }
            else if(userIsPPRBReviewer)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToOrg(user.OrgId.Value));
            }
            else if(userIsAnSCReviewer)
            {
                return convertCorrectiveActionListItems(RetrieveAllByCreatedAtOrg(user.OrgId.Value));
            }
            else
            {
                throw new Exception("Permission to View By Organization not found");
            }
        }

        /// <summary>
        /// For SC, BRC or PPRM Reviewer Only
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<CorrectiveActionListItem> RetrieveAllForOrganizationArchive(User user)
        {
            bool userIsAnSCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.SC_REVIEWER);
            bool userIsPPRBReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_REVIEWER);
            bool userIsAPPRMReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_REVIEWER);

            if(userIsAPPRMReviewer)
            {
                return convertCorrectiveActionListItems(retrieveAllArchive());
            }
            else if(userIsPPRBReviewer)
            {
                return convertCorrectiveActionListItems(RetrieveAllByAssignedToOrCreatedByOrgArchive(user.OrgId.Value));
            }
            else if(userIsAnSCReviewer)
            {
                return convertCorrectiveActionListItems(RetrieveAllByCreatedAtOrgArchive(user.OrgId.Value));
            }
            else
            {
                throw new Exception("Permission to View By Organization not found");
            }


             
        }        

        /// <summary>
        /// SC SPECIALIST
        /// </summary>
        /// <param name="CreatedByUser"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByCreatedBy(int createdByUserId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.CreatedByUserId == createdByUserId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;   
        }


        /// <summary>
        /// SC SPECIALIST
        /// </summary>
        /// <param name="CreatedByUser"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByCreatedByArchive(int createdByUserId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.CreatedByUserId == createdByUserId && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;   
        }


        /// <summary>
        /// SC REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByCreatedAtOrg(int createdAtOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.CreatedAtOrgId == createdAtOrgId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
//                         where recs.CreatedAtOrgId == createdAtOrgId
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }         


        /// <summary>
        /// SC REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByCreatedAtOrgArchive(int createdAtOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.CreatedAtOrgId == createdAtOrgId && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
//                         where recs.CreatedAtOrgId == createdAtOrgId
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }       

        /// <summary>
        /// PPRB REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrCreatedByOrg(int orgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.AssignedToOrgId == orgId || recs.CreatedAtOrgId == orgId) && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 
        /// <summary>
        /// PPRB REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrCreatedByUserId(int userId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.AssignedToUserId == userId || recs.CreatedByUserId == userId) && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse) 
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }


        /// <summary>
        /// PPRB REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
//        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrCreatedByUserIdArchive(int userId)
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrCreatedByUserIdArchive(int userId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.AssignedToUserId == userId || recs.CreatedByUserId == userId) && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse) 
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;              
        }        

        /// <summary>
        ///  PPRM || PPRB Specialist
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToUser(int assignedToUserId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.AssignedToUserId == assignedToUserId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 

        /// <summary>
        /// PPRM REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrg(int AssignedToOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.AssignedToOrgId == AssignedToOrgId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),                             
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }


        public List<CorrectiveActionListItem> RetrieveAllArchive()
        {
            return convertCorrectiveActionListItems(retrieveAllArchive());
        }

        private IQueryable<CorrectiveAction> retrieveAllArchive()
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        /// <summary>
        /// PPRB REVIEWER
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> RetrieveAllByAssignedToOrCreatedByOrgArchive(int orgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.AssignedToOrgId == orgId || recs.CreatedAtOrgId == orgId) && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }

        public List<CorrectiveActionListItem> RetrieveAll()
        {
            return convertCorrectiveActionListItems(retrieveAll());
        }


        private IQueryable<CorrectiveAction> retrieveAll()
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                 
        }

        public List<CorrectiveActionListItem> RetrieveAllByEmployeePOID(User user)
        {
            return convertCorrectiveActionListItems(retrieveAllByEmployeePOID(user));
        }


        private IQueryable<CorrectiveAction> retrieveAllByEmployeePOID(User user)
        {
            string personnelOfficerIdentifier = "9999";
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            try
            {
                personnelOfficerIdentifier = referenceRepository.RetrievePoidByOrgCode(user.OrgId.Value).PoiId.ToString();
            }
            catch (System.Exception)
            {
            }
            int userOrgId = user.OrgId.Value;
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.Empl.PersonnelOfficeIdentifier == personnelOfficerIdentifier && recs.CreatedAtOrgId != userOrgId) && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 


        public List<CorrectiveActionListItem> RetrieveAllByEmployeePOIDArchive(User user)
        {
            return convertCorrectiveActionListItems(retrieveAllByEmployeePOIDArchive(user));
        }

        /// <summary>
        ///  PPRM || PPRB Specialist
        /// </summary>
        /// <param name="AssignedToOrgId"></param>
        /// <returns></returns>
        private IQueryable<CorrectiveAction> retrieveAllByEmployeePOIDArchive(User user)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            string personnelOfficerIdentifier = "9999";
            try
            {
                personnelOfficerIdentifier = referenceRepository.RetrievePoidByOrgCode(user.OrgId.Value).PoiId.ToString();
            }
            catch (System.Exception)
            {
            }
            int userOrgId = user.OrgId.Value;
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where (recs.Empl.PersonnelOfficeIdentifier == personnelOfficerIdentifier && recs.CreatedAtOrgId != userOrgId) && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        }

        public List<CorrectiveActionListItem> RetrieveAgingReport(User user)
        {
            return convertCorrectiveActionListItems(retrieveAgingReport(user));
        }

        private IQueryable<CorrectiveAction> retrieveAgingReport(User user)
        {
            int daysBack = -7;
            int OrgIdToFind = user.OrgId.Value;
            bool userIsAnSCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.SC_SPECIALIST);
            bool userIsPPRBSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_SPECIALIST);
            bool userIsAPPRMSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_SPECIALIST);

            bool userIsAnSCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.SC_REVIEWER);
            bool userIsPPRBReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_REVIEWER);
            bool userIsAPPRMReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_REVIEWER);

            if(userIsAPPRMReviewer ||userIsAPPRMSpecialist )
            {
                daysBack = -14;
            }

            if(userIsAnSCSpecialist || userIsPPRBSpecialist || userIsAPPRMSpecialist)
            {
                return RetrieveAgingReportByUser(daysBack,OrgIdToFind);
            }
            else if(userIsAnSCReviewer || userIsPPRBReviewer || userIsAPPRMReviewer)
            {
                return RetrieveAgingReportByOrganization(daysBack,OrgIdToFind);
            }
            else{
                return null;
            }

        }

        private IQueryable<CorrectiveAction> RetrieveAgingReportByUser(int daysBack, int orgToFind)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.DeletedAt == null && recs.AssignedToOrgId == orgToFind && recs.AssignedAt.Value <= dateToUse && recs.ResolvedAt == null
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 


        } 



        public IQueryable<CorrectiveAction> RetrieveAgingReportByOrganization(int daysBack, int orgToFind)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         orderby recs.Id
                         where recs.DeletedAt == null && recs.AssignedToOrgId == orgToFind && recs.AssignedAt.Value <= dateToUse && recs.ResolvedAt == null
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             NOACode = recs.NatureOfAction,
                             ActionType = new ActionType(recs.ActionRequestType),
                             NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false, false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 


        } 

#region "Used by Notification Sender"

   public IQueryable<CorrectiveAction> RetrieveAllByStatusAndAge(int statusId, int daysold)
        {
            int negDay = daysold - (daysold * 2);
            DateTime dateToUse = DateTime.Now.AddDays(negDay);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         where recs.StatusId == statusId && recs.DeletedAt == null && recs.ResolvedAt == null && recs.UpdatedAt.Value < dateToUse
                         orderby recs.AssignedToOrgId
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             //NOACode = recs.NatureOfAction,
                             //ActionType = new ActionType(recs.ActionRequestType),
                             //NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUserId = recs.CreatedByUserId,
                             //CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             //AssignedToOrg = new Organization(recs.AssignedToOrg),
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             //AssignedToUser = new User(recs.AssignedToUser,false,false),
                             StatusId = recs.StatusId,
                             //Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 

        public IQueryable<CorrectiveAction> RetrieveAllByStatusAndAge(int statusId, int daysold, int orgId)
        {
            int negDay = daysold - (daysold * 2);
            DateTime dateToUse = DateTime.Now.AddDays(negDay);
            var retval = from recs in correctiveActionRepository.RetrieveAll()
                         where recs.AssignedToOrgId == orgId && recs.StatusId == statusId && recs.DeletedAt == null && recs.ResolvedAt == null && recs.UpdatedAt.Value < dateToUse
                         orderby recs.AssignedToOrgId
                         select new CorrectiveAction{
                             Id = recs.Id,
                             ActionRequestTypeId = recs.ActionRequestTypeId,
                             EmplId = recs.EmplId,
                             //NOACode = recs.NatureOfAction,
                             //ActionType = new ActionType(recs.ActionRequestType),
                             //NatureOfAction = new NatureOfAction(recs.NatureOfActionNavigation),
                             EffectiveDateOfPar = recs.EffectiveDateOfPar,
                             paymentMismatch = recs.IsPaymentMismatch,
                             ParEffectiveDate = recs.PareffectiveDate,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUserId = recs.CreatedByUserId,
                             //CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             //AssignedToOrg = new Organization(recs.AssignedToOrg),
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             //AssignedToUser = new User(recs.AssignedToUser,false,false),
                             StatusId = recs.StatusId,
                             //Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             Details = recs.Details,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 



#endregion

    }
}