using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QmsCore.Model;
using QmsCore.QmsException;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Services;

namespace QmsCore.Engine
{
    internal class DataErrorRoutingEngine : BaseRoutingEngine
    {
        DataError entity;
        DataErrorService dataErrorService;
        DataErrorRepository dataErrorRepository;
        ReferenceService referenceService;

        CorrectiveActionService correctiveActionService;
        internal DataErrorRoutingEngine(DataErrorService service)
        {
            dataErrorService = service;
            referenceRepository = new ReferenceRepository();
            referenceService = new ReferenceService();
            correctiveActionService = new CorrectiveActionService();
            dataErrorRepository = service.repository;
        }

        internal int ExecuteUpdates(DataError ehriError, User submittedBy, string history)
        {
            submitter = submittedBy;
            entity = ehriError;
            StatusTransition action = referenceRepository.RetrieveOrgStatusTranstion(ehriError.OrgStatusTransId);
            string actionStatus = action.ToStatus.StatusCode;
            entity.StatusId = action.ToStatus.StatusId;
            entity.UpdatedAt = DateTime.Now;

            if(actionStatus == StatusType.UNASSIGNED)
            {
                ActionDescription = string.Format("{0} submitted error for resolution<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = EhriErrorNotificationType.EHRI_Submitted;
                SubmitForResolution();
            }
            else if(actionStatus == StatusType.ASSIGNED)
            {
                ActionDescription = string.Format("{0} assigned error<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = EhriErrorNotificationType.EHRI_Assigned;
                AssignToUser(entity);
            }
            else if(actionStatus == StatusType.CLOSED)
            {
                ActionDescription = string.Format("{0} closed error<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = EhriErrorNotificationType.EHRI_Closed;
                Close(entity);
            }
            else if(actionStatus == StatusType.CLOSED_ACTION_COMPLETED)
            {
                ActionDescription = string.Format("{0} completed error<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = EhriErrorNotificationType.EHRI_Closed;
                CloseActionCompleted(entity);  
            }
            else if(actionStatus == StatusType.PENDING_REVIEW)
            {
                ActionDescription = string.Format("{0} submitted error for review<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = EhriErrorNotificationType.EHRI_PendingReview;
                SubmitForReview(entity);
            }
            else if(actionStatus == StatusType.RETURNED)
            {
                ActionDescription = string.Format("{0} returned error to specialist<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription);             
                NotificationEventType = EhriErrorNotificationType.EHRI_Returned;
                Return(entity);                
            }
            else if (actionStatus == StatusType.CLOSED_CONVERT_TO_CORR_ACTION)
            {
                ActionDescription = string.Format("{0} converted to corrective action<br/>{1}",submitter.DisplayName,history);
                dataErrorService.addHistory(this.entity, submitter, ActionDescription); 
                NotificationEventType = EhriErrorNotificationType.EHRI_Closed;            
                createCorrectiveAction(submittedBy);
            }

            QmsDataerror error = entity.QmsDataError();
            dataErrorRepository.Update(error);
            dataErrorRepository.context.SaveChanges();

            return ehriError.Id;
        }

        internal new void SubmitForReview(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.PENDING_REVIEW).StatusId; 
        }

        internal new void Return(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.RETURNED).StatusId;            
        }

        private void SubmitForResolution()
        {
            entity.AssignedToUserId = null;
            entity.AssignedAt = null;
            entity.AssignedByUserId = null;
            entity.AssignedToOrgId = getOrg("PPRM").OrgId;            
        }




//        internal int createCorrectiveAction(EhriError ehriError, User submittedBy)
        private void createCorrectiveAction(User submittedBy)
        {
            //create corrective action
            CorrectiveAction correctiveAction = new CorrectiveAction();
            correctiveAction.EmplId = entity.EmplId;
            correctiveAction.Employee = entity.Employee;          
            correctiveAction.NOACode = "000";
            correctiveAction.NatureOfAction = referenceService.RetrieveNatureOfAction("000");
            correctiveAction.Details = entity.Details;
            correctiveAction.EffectiveDateOfPar = DateTime.Now;
            correctiveAction.ParEffectiveDate = null;
            correctiveAction.IsPaymentMismatch=false;
            correctiveAction.ErrorTypes = new System.Collections.Generic.List<ErrorType>();
            correctiveAction.ActionId = 37;  //37 Save As Draft
            correctiveAction.CreatedAtOrgId = submittedBy.OrgId.Value;
            correctiveAction.CreatedByUserId = submittedBy.UserId;
            correctiveAction.ActionRequestTypeId = 1;
            correctiveAction.RowVersion = 1;
            int caId = correctiveActionService.Save(correctiveAction,submittedBy);

            //create corrective action history item to denote it came from an EHRI error
            correctiveAction.Id = caId;
            string message = string.Format("Corrective Action Created from EHRI Error {0} by {1},<br/><br/>Details:<br/>{2}",entity.Id,submittedBy.DisplayName,entity.QmsErrorMessageText);
            correctiveActionService.addHistory(correctiveAction,submittedBy,message);

            //close out ehri error
            entity.CorrectiveActionId = caId;
            entity.ResolvedAt = DateTime.Now;
        }
    }//end class
}//end namespace