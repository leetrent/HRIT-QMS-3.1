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
    internal class CorrectiveActionRoutingEngine : BaseRoutingEngine
    {
        CorrectiveAction entity;

        CorrectiveActionRepository correctiveActionRepository;

        CorrectiveActionService correctiveActionService;

        bool routesToRBC;        

        private bool determineIfTicketRoutesToRBC()
        {
            bool routesToRBCCheck = entity.NatureOfAction.RoutesToBr == "Y";
            if(!routesToRBCCheck)
            {
                foreach(ErrorType errorType in entity.ErrorTypes)
                {
                    if(errorType.RoutesToBR == "Y")
                    {
                        routesToRBCCheck=true;
                        break;
                    }
                }
            }
            return routesToRBCCheck;
        }

        internal CorrectiveActionRoutingEngine(CorrectiveActionService caService)
        {
           correctiveActionService = caService;
           referenceRepository = caService.referenceRepository;
           correctiveActionRepository = caService.correctiveActionRepository;

        }

        internal int ExecuteUpdates(CorrectiveAction correctiveAction, User submittedBy, string history)
        {
            submitter = submittedBy;
            this.entity = correctiveAction;
            int retval = entity.Id;
            isNewTicket = retval == 0;
            routesToRBC = determineIfTicketRoutesToRBC(); // if submitted by PPRM this would need to be skipped
            StatusTransition action = referenceRepository.RetrieveOrgStatusTranstion(correctiveAction.ActionId);
            string actionToStatus = action.ToStatus.StatusCode;
            this.entity.StatusId = action.ToStatus.StatusId;
            if(isNewTicket)
            {
                this.entity.CreatedAt = DateTime.Now;
                this.entity.CreatedByUserId = submitter.UserId;
                this.entity.CreatedAtOrgId= submitter.OrgId.Value;
                this.entity.AssignedToOrgId = submitter.OrgId.Value;
                QmsCorrectiveactionrequest initialCar = correctiveAction.QmsCorrectiveActionRequest;
                correctiveActionRepository.context.Add(initialCar);
                correctiveActionRepository.context.SaveChanges();
                retval = initialCar.Id;
                this.entity.Id = initialCar.Id;
            }
            else
            {
                this.entity.UpdatedAt = DateTime.Now;
            }

            

            if(actionToStatus == StatusType.UNASSIGNED)
            {
                ActionDescription = string.Format("{0} submitted Corrective Action for resolution<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_Submitted;
                SubmitForResolution();
            }
            else if(actionToStatus == StatusType.ASSIGNED)
            {
                ActionDescription = string.Format("{0} assigned Corrective Action<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_Assigned;
                AssignToUser(entity);
            }
            else if(actionToStatus == StatusType.CLOSED)
            {
                ActionDescription = string.Format("{0} closed Corrective Action<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_Closed;
                Close(entity);
            }
            else if (actionToStatus == StatusType.CLOSED_ACTION_COMPLETED)
            {
                ActionDescription = string.Format("{0} completed Corrective Action<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_Closed;
                CloseActionCompleted(entity);                
            }
            else if(actionToStatus == StatusType.DRAFT)
            {
                if(isNewTicket)
                {
                    ActionDescription = string.Format("{0} created Corrective Action",submitter.DisplayName);
                    correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                    NotificationEventType = CorrectiveActionNotificationType.CA_Created;
                    SaveAsDraft(entity);
                }
                else
                {
                    ActionDescription = string.Format("{0} withdrew Corrective Action<br/>{1}",submitter.DisplayName,history);
                    correctiveActionService.addHistory(this.entity, submitter, ActionDescription);                    
                    NotificationEventType = CorrectiveActionNotificationType.CA_Withdrawn;
                    WithdrawItem(entity);                        
                }
            }
            else if(actionToStatus == StatusType.PENDING_REVIEW)
            {
                ActionDescription = string.Format("{0} submitted Corrective Action for review<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_PendingReview;
                SubmitForReview(entity);
            }                
            else if(actionToStatus == StatusType.RETURNED)
            {
                ActionDescription = string.Format("{0} returned Corrective Action to originator<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);             
                NotificationEventType = CorrectiveActionNotificationType.CA_Returned;
                Return(entity);
            }   
            else if (actionToStatus == StatusType.REROUTED)
            {
                ActionDescription = string.Format("{0} rerouted Corrective Action<br/>{1}",submitter.DisplayName,history);
                correctiveActionService.addHistory(this.entity, submitter, ActionDescription);
                NotificationEventType = CorrectiveActionNotificationType.CA_Rerouted;

                Reroute(entity);
            }   
            else
            {
                throw new InvalidStatusTypeException(actionToStatus + " is not a valid StatusType.");
            }
            QmsCorrectiveactionrequest car = this.entity.QmsCorrectiveActionRequest;
            correctiveActionRepository.Update(car);
            saveErrors(this.entity.ErrorTypes, retval, isNewTicket);
            correctiveActionRepository.context.SaveChanges();  

            return retval;
        }



        private void saveErrors(List<ErrorType> errorTypes, int entityId, bool isNewTicket)
        {

            if(!isNewTicket)
            {
                var oldErrors = correctiveActionRepository.context.QmsCorrectiveactionErrortype.Where(e => e.CorrectiveActionId == entityId);
                correctiveActionRepository.context.QmsCorrectiveactionErrortype.RemoveRange(oldErrors);
                correctiveActionRepository.context.SaveChanges();
            }
 
            foreach(ErrorType errorType in errorTypes)
            {
                QmsCorrectiveactionErrortype actionErrorType = new QmsCorrectiveactionErrortype();
                actionErrorType.ErrorTypeId = errorType.Id.Value;
                actionErrorType.CorrectiveActionId = entityId;
                correctiveActionRepository.context.Add(actionErrorType);
            }    
          
        }

          

        private void SubmitForResolution()
        {
            SecOrg PPRB = getOrg(RBCCode);
            SecOrg PPRM = getOrg(PPRMCode);
            int currentOrgId = entity.AssignedToOrgId.HasValue ? entity.AssignedToOrgId.Value : submitter.OrgId.Value;
            bool isAtPPRB = (currentOrgId == PPRB.OrgId);
            Employee emp = new EmployeeService().RetrieveById(entity.EmplId);
            entity.AssignedToUserId = null;
            entity.AssignedAt = null;
            entity.AssignedByUserId = null;
            entity.StatusId = referenceRepository.GetStatus(StatusType.UNASSIGNED).StatusId;

            if(routesToRBC && !isAtPPRB && emp.PersonnelOfficeIdentifier != "4008")//Don't send CABS employees to BRC per Jamie Hamlin
            {
                entity.AssignedToOrgId = PPRB.OrgId;
            }
            else
            {
                entity.AssignedToOrgId = PPRM.OrgId;    
            }

        }      








    }
}