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

    internal class BaseRoutingEngine
    {

        internal User submitter;
        internal ReferenceRepository referenceRepository;

        internal string RBCCode = "BRC";
        internal string PPRMCode = "PPRM";
        internal string ActionDescription = "";

        public string NotificationEventType = "";
        internal QmsWorkitemhistory itemHistory;
        internal bool isNewTicket;


        internal SecOrg getOrg(string orgCode)
        {
            return referenceRepository.context.SecOrg.AsNoTracking().Where(o => o.OrgCode == orgCode).SingleOrDefault();

        }
        

        internal void createWorkItemHistory(IListable entity)
        {
            itemHistory = new QmsWorkitemhistory();
            itemHistory.ActionTakenByUserId = submitter.UserId;
            itemHistory.CreatedAt = DateTime.Now;
            itemHistory.WorkItemId = entity.Id;
            itemHistory.ActionTakenByUserId = submitter.UserId;
            itemHistory.WorkItemTypeCode = entity.WorkItemType;
            itemHistory.ActionDescription = ActionDescription;
            referenceRepository.context.Add(itemHistory);
        }        

        internal void SaveAsDraft(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.DRAFT).StatusId;
            entity.AssignedAt = null;
            entity.AssignedByUserId = null;
            entity.AssignedToUserId = null;
            entity.AssignedToOrgId = submitter.OrgId;
        }    

        internal virtual void SubmitForReview(IListable entity)
        {
            entity.AssignedToOrgId = submitter.OrgId;
            entity.StatusId = referenceRepository.GetStatus(StatusType.PENDING_REVIEW).StatusId;
            entity.AssignedToUserId = null;
            entity.AssignedAt = null;
        }


        internal void AssignToUser(IListable entity)
        {

            entity.StatusId = referenceRepository.GetStatus(StatusType.ASSIGNED).StatusId;            
            entity.AssignedAt = DateTime.Now;
            entity.AssignedByUserId = submitter.UserId;
            entity.AssignedToOrgId = submitter.OrgId;
        }

        internal void Reroute(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.REROUTED).StatusId;            
            entity.AssignedAt = null;
            entity.AssignedToUserId = null;
            entity.AssignedByUserId = null;
            entity.AssignedToOrgId = getOrg(RBCCode).OrgId;
        }

  
        internal void Close(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.CLOSED).StatusId;            
            entity.ResolvedAt = DateTime.Now;
        }

        internal void CloseActionCompleted(IListable entity)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.CLOSED_ACTION_COMPLETED).StatusId;            
            entity.ResolvedAt = DateTime.Now;
        }

        internal void ReOpen(IListable entity,User reopenedBy, User assignedTo)
        {
            entity.StatusId = referenceRepository.GetStatus(StatusType.ASSIGNED).StatusId;            
            entity.AssignedByUserId = reopenedBy.UserId;
            entity.AssignedToUserId = assignedTo.UserId;
            entity.AssignedToOrgId = assignedTo.OrgId;
            entity.AssignedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.ResolvedAt = null;
            entity.DeletedAt = null;
        }

        internal void Return(IListable entity)
        {
            
            SecUser originator = this.referenceRepository.context.SecUser.Where(u => u.UserId == entity.CreatedByUserId).SingleOrDefault();

            entity.AssignedToUserId = null;
            entity.AssignedToOrgId = originator.OrgId.Value;
            entity.AssignedByUserId = null;
            entity.StatusId = referenceRepository.GetStatus(StatusType.RETURNED).StatusId;            
            entity.AssignedAt = null;
            entity.ResolvedAt = null;
        }


        internal void WithdrawItem(IListable entity)
        {
            entity.AssignedToUserId = submitter.UserId;
            entity.AssignedToOrgId = submitter.OrgId;
            entity.AssignedByUserId = submitter.UserId;
            entity.StatusId = referenceRepository.GetStatus(StatusType.DRAFT).StatusId;            
            entity.AssignedAt = null;
            entity.ResolvedAt = null;
        }  

        


    }//end class
}//end namespace