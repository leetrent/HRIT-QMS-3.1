using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Engine;
using QmsCore.QmsException;
using System.Text;
using QmsCore.Lib;

namespace QmsCore.Services
{
    public class DataErrorService : BaseService<DataError>, IDataErrorService
    {
        internal DataErrorRepository repository;
        internal ReferenceService referenceService;
        internal int archiveDayCount = -30; //-30

#region "Constructor's"
        public DataErrorService()
        {
            repository = new DataErrorRepository(this.context);
            referenceService = new ReferenceService(this.context);
        }

        public DataErrorService(QMSContext qmsContext)
        {
            this.context = qmsContext;
            repository = new DataErrorRepository(this.context);
            referenceService = new ReferenceService(this.context);            
        }
#endregion

#region "Update methods"

        public void SaveComment(string message
                                      ,int entityId
                                      ,int authorId)
        {
            DataErrorComment comment = new DataErrorComment(message,entityId,authorId);
            QmsWorkitemcomment workItemComment = comment.WorkItemComment();
            this.repository.context.Add(workItemComment);
            this.repository.Save();
        }


        public int? Save(int errorId, string details, int actionId, User submitter, int? assigneeId)
        {
            DataError d = new DataError(repository.RetrieveById(errorId));
            d.Details = details;
            d.OrgStatusTransId = actionId;
            if(assigneeId.HasValue)
            {
                d.AssignedByUserId = submitter.ID;
                d.AssignedToUserId = assigneeId;
                d.AssignedAt = DateTime.Now;
            }
    
            return Save(d,submitter);
        }

        // Save returns teh value of the associated Corrective Action which is an nullable ihnt (int?) type. 
        // The 
        //
        private int? Save(DataError newVersion, User submitter)
        {
            string history = string.Empty;
            DataError oldVersion = RetrieveById(newVersion.Id);
            if(newVersion.Id > 0) // exiting EhriError
            {
                if(newVersion.RowVersion == oldVersion.RowVersion)
                {
                    history = writeHistory(newVersion,oldVersion,submitter);
                    newVersion.RowVersion++;
                    DataErrorRoutingEngine ehriErrorRoutingEngine = new DataErrorRoutingEngine(this);
                    ehriErrorRoutingEngine.ExecuteUpdates(newVersion,submitter,history); 
                    DataErrorNotificationEngine ehriErrorNotificationEngine = new DataErrorNotificationEngine(); 
                    ehriErrorNotificationEngine.Send(newVersion,ehriErrorRoutingEngine.NotificationEventType, submitter);                      

                }
                else
                {
                    throw new LockingException(string.Format("This EHRI Error {0} was updated by another user. Old version #{1}, new version #{2}",newVersion.Id, newVersion.RowVersion, oldVersion.RowVersion));
                }

            }
            else //new EhriError
            {

            }
            return newVersion.CorrectiveActionId;

        }


         private string writeHistory(DataError newVersion, DataError oldVersion, User submitter)
         {
            StringBuilder sb = new StringBuilder();
            int changeCount = 0;
            string lineBreak = "<br/>";
            if(newVersion.Details != oldVersion.Details)
            {
               changeCount++;
               sb.AppendLine(string.Format(" - Details changed from: {2} {0} to {2} {1} {2}",oldVersion.Details,newVersion.Details,lineBreak));            
            }        
            return sb.ToString();
         }



#endregion
#region "Retrieve Methods"

    public DataError RetrieveById(int id, User searcher)
    {
        var item = repository.RetrieveById(id);
        DataError retval = new DataError(item,searcher);
        retval.Comments = RetrieveComments(id);
        retval.Histories = retrieveHistory(id);        
        addSearchHistory(item,searcher);
        return retval;
    }

    internal DataError RetrieveById(int id)
    {
        var item = repository.RetrieveById(id);
        DataError retval = new DataError(item);
        return retval;
    }    


        public List<DataErrorComment> RetrieveComments(int DataErrorId)
        {
            bool enableUserSecurityLoading = false;
            var items = context.QmsWorkitemcomment.AsNoTracking().Where(c => c.WorkItemId == DataErrorId && c.WorkItemTypeCode == QmsCore.Model.WorkItemTypeEnum.EHRI).Include(c => c.WorkItemTypeCodeNavigation).Include(c => c.Author).ThenInclude(u => u.Org);
            List<DataErrorComment> comments = new List<DataErrorComment>();
            foreach(var item in items)
            {
                comments.Add(new DataErrorComment(item,enableUserSecurityLoading));
            }
            return comments;
        }

        private List<DataErrorHistory> retrieveHistory(int DataErrorId)
        {
            var items = context.QmsWorkitemhistory.AsNoTracking().Where(c => c.WorkItemId == DataErrorId && c.WorkItemTypeCode == QmsCore.Model.WorkItemTypeEnum.EHRI).Include(c => c.PreviousAssignedByUser).Include(c => c.PreviousAssignedToOrg).Include(c => c.PreviousStatus).Include(c => c.PreviousAssignedtoUser).Include(c => c.ActionTakenByUser).Include(c => c.WorkItemTypeCodeNavigation); 
            List<DataErrorHistory> histories = new List<DataErrorHistory>();
            foreach(var item in items)
            {
                histories.Add(new DataErrorHistory(item));
            }
            return histories;
        }

        private List<DataErrorListItem> convertToListItems(IQueryable<DataError> errors)
        {
            List<DataErrorListItem> retval = new List<DataErrorListItem>();
            foreach(var error in errors)
            {
                DataErrorListItem item = error.DataErrorListItem();
                retval.Add(item);
            }                
            return retval;
        }


        public List<DataErrorListItem> RetrieveByActionCode(User user,string actionCode)
        {
            switch(actionCode)
            {
                case "VDEFU":
                    return RetrieveAllForUser(user);
                case "VADEFU":
                    return RetrieveAllForUserArchive(user);
                case "VDEFO":
                    return RetrieveAllByOrganization(user);    
                case "VADEFO":
                    return RetrieveAllByOrganizationArchive(user);
                case "VDEALL":
                    return RetrieveAll();
                case "VADEALL":    
                    return RetrieveAllArchive();
                case "VADAGE":
                    //return RetrieveAgingReportForUser(user);
                default:
                    throw new Exception("Unable to determine requested action: " + actionCode);
            }
        }


        public List<DataErrorListItem> RetrieveAllForUser(User user)
        {
            string role = "UNKNOWN";

            role = user.UserRoles[0].Role.RoleCode;

            switch (role)
            {
                case ApplicationRoleType.SC_SPECIALIST:
                case ApplicationRoleType.PPRM_SPECIALIST:
                case ApplicationRoleType.PPRB_SPECIALIST:
                    return convertToListItems(RetrieveAllByAssignedToUser(user.UserId));
                default:
                    throw new Exception("Role '" + role + "' not found");
            }
        }


        public List<DataErrorListItem> RetrieveAllForUserArchive(User user)
        {
            string role = "UNKNOWN";

            role = user.UserRoles[0].Role.RoleCode;

            switch (role)
            {
                case ApplicationRoleType.SC_SPECIALIST:
                case ApplicationRoleType.PPRM_SPECIALIST:
                case ApplicationRoleType.PPRB_SPECIALIST:
                    return convertToListItems(RetrieveAllByAssignedToUserArchive(user.UserId));
                default:
                    throw new Exception("Role '" + role + "' not found");
            }                

        }        

        public List<DataErrorListItem> RetrieveAllByOrganization(User user)
        {
            string role = "UNKNOWN";

            role = user.UserRoles[0].Role.RoleCode;

            switch (role)
            {
                case ApplicationRoleType.SC_REVIEWER:
                    return convertToListItems(RetrieveAllByCreatedAtOrg(user.OrgId.Value));
                case ApplicationRoleType.PPRM_REVIEWER:
                    return convertToListItems(RetrieveAllByAssignedToOrg(user.OrgId.Value));
                case ApplicationRoleType.PPRB_REVIEWER:
                    return convertToListItems(RetrieveAllByAssignedToOrCreatedByOrg(user.OrgId.Value));
                default:
                    throw new Exception("Role '" + role + "' not found");
            }                

        }

        public List<DataErrorListItem> RetrieveAllByOrganizationArchive(User user)
        {
            string role = "UNKNOWN";
            role = user.UserRoles[0].Role.RoleCode;

            switch (role)
            {
                case ApplicationRoleType.SC_REVIEWER:
                    return convertToListItems(RetrieveAllByCreatedAtOrgArchive(user.OrgId.Value));
                case ApplicationRoleType.PPRM_REVIEWER:
                    return convertToListItems(RetrieveAllByAssignedToOrgArchive(user.OrgId.Value));
                case ApplicationRoleType.PPRB_REVIEWER:
                    return convertToListItems(RetrieveAllByAssignedToOrCreatedByOrgArchive(user.OrgId.Value));
                default:
                    throw new Exception("Role '" + role + "' not found");
            }                
             
        } 

        private IQueryable<DataError> RetrieveAllByCreatedAtOrg(int createdAtOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.CreatedByOrgId == createdAtOrgId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };

            return retval;                         
        }              

        private IQueryable<DataError> RetrieveAllByCreatedAtOrgArchive(int createdAtOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.CreatedByOrgId == createdAtOrgId && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             SubmittedAt = recs.SubmittedAt,
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        }      


        private IQueryable<DataError> RetrieveAllByAssignedToOrg(int AssignedToOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.AssignedToOrgId == AssignedToOrgId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),                             
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToOrgArchive(int AssignedToOrgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.AssignedToOrgId == AssignedToOrgId && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),                             
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToOrCreatedByOrg(int orgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where (recs.AssignedToOrgId == orgId || recs.CreatedByOrgId == orgId) && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToOrCreatedByOrgArchive(int orgId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where (recs.AssignedToOrgId == orgId || recs.CreatedByOrgId == orgId) && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToOrCreatedByUserId(int userId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where (recs.AssignedToUserId == userId || recs.CreatedByUserId == userId) && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse) 
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval;                         
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToUser(int assignedToUserId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.AssignedToUserId == assignedToUserId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 

        private IQueryable<DataError> RetrieveAllByAssignedToUserArchive(int assignedToUserId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.AssignedToUserId == assignedToUserId && recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return retval; 
        } 


        public List<DataErrorListItem> RetrieveAll()
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
//                         where recs.AssignedToUserId == assignedToUserId && recs.DeletedAt == null && (recs.ResolvedAt != null || recs.ResolvedAt.Value <= DateTime.Now.AddDays(archiveDayCount))            
                         where recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return convertToListItems(retval);                 
        }


        public List<DataErrorListItem> RetrieveAllArchive()
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
//                         where recs.AssignedToUserId == assignedToUserId && recs.DeletedAt == null && (recs.ResolvedAt != null || recs.ResolvedAt.Value <= DateTime.Now.AddDays(archiveDayCount))            
                         where recs.DeletedAt == null && (recs.ResolvedAt != null && recs.ResolvedAt.Value <= dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             UpdatedAt = recs.UpdatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return convertToListItems(retval);   
        } 


        public List<DataErrorListItem> RetrieveAllByEmployee(string employeeId)
        {
            DateTime dateToUse = DateTime.Now.AddDays(archiveDayCount);
            var retval = from recs in repository.RetrieveAll()
                         where recs.Emplid == employeeId && recs.DeletedAt == null && (recs.ResolvedAt == null || recs.ResolvedAt.Value > dateToUse)
                         select new DataError{
                             Id = recs.Id,
                             EmplId = recs.Emplid,
                             CorrectiveActionId = recs.CorrectiveActionId,
                             CreatedAt = recs.CreatedAt,
                             CreatedByUser = new User(recs.CreatedByUser,false,false),                             
                             UpdatedAt = recs.UpdatedAt,
                             ResolvedAt = recs.ResolvedAt,
                             AssignedToOrg = new Organization(recs.AssignedToOrg),
                             Employee = new Employee(recs.Empl),
                             AssignedToOrgId = recs.AssignedToOrgId,
                             AssignedByUserId = recs.AssignedByUserId,
                             AssignedToUserId = recs.AssignedToUserId,
                             AssignedToUser = new User(recs.AssignedToUser,false),
                             StatusId = recs.StatusId,
                             Status = new Status(recs.Status),
                             DataCategory = recs.ErrorList.DataItem.DataItemCategory,
                             DataElement = recs.ErrorList.DataItem.DataItemName,
                             SubmittedAt = recs.SubmittedAt,
                             DataErrorId = recs.DataErrorId,
                             DataErrorKey = recs.DataErrorKey,
                             QmsErrorCode = recs.QmsErrorCode,
                             QmsErrorMessageText = recs.QmsErrorMessageText,
                             DeletedAt = recs.DeletedAt
                         };
            return convertToListItems(retval);   
        } 

#endregion

    }//end class
}//end namespace