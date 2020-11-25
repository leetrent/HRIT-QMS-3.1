using System;
using System.Collections.Generic;

namespace QmsCore.UIModel
{
    public class CorrectiveActionHistory : IComparable<CorrectiveActionHistory>
    {
        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public string WorkItemType { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string ActionDescription {get;set;}
        public int? ActionTakenByUserId { get; set; }
        public int? PreviousStatusId { get; set; }
        public int? PreviousAssignedToOrgId { get; set; }
        public int? PreviousAssignedtoUserId { get; set; }
        public int? PreviousAssignedByUserId { get; set; }

        public virtual User ActionTakenByUser { get; set; }
        public virtual User PreviousAssignedByUser { get; set; }
        public virtual Organization PreviousAssignedToOrg { get; set; }
        public virtual User PreviousAssignedtoUser { get; set; }
        public virtual Status PreviousStatus { get; set; }

        public CorrectiveActionHistory(QmsCore.Model.QmsWorkitemhistory history)
        {
            bool loadUserSecurity = false;
            bool loadUserOrganizationInfo = false;
            this.Id = history.Id;
            this.ActionDescription = history.ActionDescription;
            this.WorkItemId = history.WorkItemId;
            this.WorkItemType = history.WorkItemTypeCodeNavigation.WorkItemTypeCode;
            this.CreatedAt = history.CreatedAt;
            this.ActionTakenByUserId = history.ActionTakenByUserId;
            this.PreviousStatusId = history.PreviousStatusId;
            this.PreviousAssignedToOrgId = history.PreviousAssignedToOrgId;
            this.PreviousAssignedtoUserId = history.PreviousAssignedtoUserId;
            this.PreviousAssignedByUserId = history.PreviousAssignedByUserId;

            if(this.PreviousStatusId.HasValue)
            {
                this.PreviousStatus = new Status(history.PreviousStatus);
            }
            else
            {
                this.PreviousStatus = new Status() {StatusId = -1, StatusCode = "DRAFT", StatusLabel = "Draft"};
            }

            if(this.ActionTakenByUserId.HasValue)
            {
                this.ActionTakenByUser = new User(history.ActionTakenByUser,loadUserSecurity,loadUserOrganizationInfo);
            }
            else
            {
                this.ActionTakenByUser = new User();
            }                


            if(this.PreviousAssignedToOrgId.HasValue)
            {
                this.PreviousAssignedToOrg = new Organization(history.PreviousAssignedToOrg);
            }
            else
            {
                this.PreviousAssignedToOrg = new Organization();
            }                

            if(this.PreviousAssignedtoUserId.HasValue)
            {
                this.PreviousAssignedtoUser = new User(history.PreviousAssignedtoUser,loadUserSecurity,loadUserOrganizationInfo);                
            }
            else
            {
                this.PreviousAssignedtoUser = new User();
            }                

            if(this.PreviousAssignedByUserId.HasValue)
            {
                this.PreviousAssignedByUser = new User(history.PreviousAssignedByUser,loadUserSecurity,loadUserOrganizationInfo);
            }
            else
            {
                this.PreviousAssignedByUser = new User();
            }
                

        }

        int IComparable<CorrectiveActionHistory>.CompareTo(CorrectiveActionHistory other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }
}
