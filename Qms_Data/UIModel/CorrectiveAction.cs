using System;
using System.Collections.Generic;
using Qms_Data.UIModel;
using QmsCore.Lib;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class CorrectiveAction : IListable, IMessageCreatable
    {
        private  int orgViewerIsFrom;

        public string Template{get {return "Corrective Acton ID: {0}<br/>Employee: {1}-{2}<br/>Request Type: {3}<br/>NOA: {4}<br/>Effective Date: {5}<br/>Updated on: {6}<br/>Correction Requested:{7}<br/>";}}


        public string Message {get {return  string.Format(Template,Id,EmplId, Employee.FullName, getActionRequestType(ActionRequestTypeId.Value) ,NOACode,EffectiveDateOfPar,getDateToUse(),Details);}}
        public int Id { get; set; }
        public int? ActionRequestTypeId { get; set; }
        public string EmplId { get; set; }
        public string NOACode { get; set; }
        public DateTime EffectiveDateOfPar { get; set; }
        internal string paymentMismatch {get; set;}
        public bool IsPaymentMismatch { get {return paymentMismatch == "Y";} 
                                        set {
                                            if(value)
                                            {
                                                paymentMismatch = "Y";
                                            }
                                            else
                                            {
                                                paymentMismatch = "N";
                                            }
                                        }
                                       }
        public DateTime? ParEffectiveDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? AssignedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? AssignedToOrgId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public int? StatusId { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public string Details { get; set; }        
        public int CreatedAtOrgId { get; set; }       
        public int ActionId { get; set; }       

        public byte RowVersion { get; set; }

        public virtual User AssignedByUser { get; set; }
        public virtual Organization AssignedToOrg { get; set; }

        public virtual Organization CreatedByOrg {get;set;}

        
        public virtual User AssignedToUser { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Status Status { get; set; }   



        public virtual ActionType ActionType {get;set;}

        private NatureOfAction natureOfAction;
        public NatureOfAction NatureOfAction{
            get{return natureOfAction;}
            set
            {
                this.NOACode = value.NoaCode;
                natureOfAction = value;
            }
        }     
        public  List<CorrectiveActionComment> Comments {get;set;}

        public List<CorrectiveActionHistory> Histories {get;set;}
        public List<ErrorType> ErrorTypes {get;set;}


        private string getActionRequestType(int actionRequestId)
        {
            if(actionRequestId == 1)
            {
                return "Correction Action";
            }
            else if(actionRequestId == 2)
            {
                return "Cancellation of Action";
            }
            else
            {
                return "Retro Action";
            }
        }

        private string getDateToUse()
        {
           return UpdatedAt.HasValue ? UpdatedAt.Value.ToShortDateString() : CreatedAt.ToShortDateString();           

        }

        public int DaysSinceCreated {
            get { //QMS-187
                    if(!this.Status.StatusLabel.Contains("Closed")) //ticket is NOT closed
                    {
                        return DateCalc.DaysBetween(this.CreatedAt, DateTime.Now);
                    }
                    else
                    {
                        return DateCalc.DaysBetween(this.CreatedAt,this.ResolvedAt.Value);
                    }
                }
        }

        public int? DaysSinceAssigned{
            get {
                if(this.AssignedAt.HasValue)
                {
                    return DateCalc.DaysBetween(this.AssignedAt.Value, DateTime.Now);
                }
                else{
                    return null;
                }
                
            }
        }

        public string WorkItemType {
            get {
                return Model.WorkItemTypeEnum.CorrectiveActionRequest;
            }
        }

        public string Priority{
           get{
               string retval = "Normal";
               if(PriorityIndex >=10)
               {
                   retval = "High";
               }
               else if(PriorityIndex >= 5)
               {
                   retval = "Elevated";
               }
               return retval;
           } 
        }

        public double PriorityIndex{
            get {

                double retval = 0;
                try{
                    double daysTotal = DaysSinceCreated / 10;
                    int daysFactor = Convert.ToInt32(daysTotal);
                    int benefitsRelatedFactor = 0;

                    int gradeFactor = 0;


                    string payPlan = this.Employee.PayPlan;
                    if(payPlan == "AD")
                        gradeFactor = 5;

                    string employeePoid = this.Employee.PersonnelOfficeIdentifier;
                    if(employeePoid == "1909")
                        gradeFactor = 5;


                    foreach(ErrorType error in this.ErrorTypes)
                    {
                        if(error.RoutesToBR == "Y")
                            benefitsRelatedFactor += 2;
                    }
                    int paymismatchFactor = 0;

                    if (this.IsPaymentMismatch)
                        paymismatchFactor = 10;

                        
                    retval = daysFactor + benefitsRelatedFactor + paymismatchFactor + gradeFactor;
                }
                catch(Exception)
                {

                }
             
                return retval;
            }
        }

        public bool IsReadOnly = true;
        private void setReadOnly(User user)
        {
            bool keepChecking = true;
            string currentStatus = this.Status.StatusCode;

            //check if it's been closed for more than 30 days
            if(currentStatus == StatusType.CLOSED || currentStatus == StatusType.CLOSED_ACTION_COMPLETED)
            {
                int daysOld = DateCalc.DaysBetween(this.ResolvedAt.Value,DateTime.Now);
                if(daysOld >= 30)
                {
                    keepChecking = false;
                    IsReadOnly = true;
                }
            }

            //check if user has read only permissions
            if(keepChecking)
            {
                IsReadOnly= ((UserUtil.UserHasPermission(user,  CorrectiveActionPermissionEnum.VIEW_ALL_CORRECTIVE_ACTIONS)) || (UserUtil.UserHasPermission(user,  CorrectiveActionPermissionEnum.VIEW_ALL_ARCHIVED_CORRECTIVE_ACTIONS)));
                keepChecking = !IsReadOnly;
            }


            //check for users with editing permissions
            if(keepChecking)
            {
                bool isSCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.SC_REVIEWER);  
                bool isSCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.SC_SPECIALIST);
                bool isBRCReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_REVIEWER);  
                bool isBRCSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRB_SPECIALIST);
                bool isPPRMReviewer = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_REVIEWER);  
                bool isPPRMSpecialist = UserUtil.UserHasRole(user,ApplicationRoleType.PPRM_SPECIALIST);                
                if(isBRCSpecialist || isSCSpecialist || isPPRMSpecialist)
                {
                    if((this.AssignedToUserId == user.UserId) || (this.CreatedByUserId == user.UserId))
                    {
                        IsReadOnly = false;  //It's assigned to them
                    }
                    else
                    {
                        IsReadOnly = true;
                    }
                }
                else if(isBRCReviewer || isSCReviewer || isPPRMReviewer)
                {
                    if((this.AssignedToOrgId == user.OrgId.Value) || (this.CreatedAtOrgId == user.OrgId.Value))
                    {
                        IsReadOnly = false;  //It's assigned to their org
                    }
                    else
                    {
                        IsReadOnly = true;
                    }
                }
                else
                {
                    IsReadOnly = true;
                }
            }
        }



        public bool IsAssignable
        {
            get{
                bool retval = false;
                try
                {
                    int i = this.StatusId.Value;
                    int? orgId  = this.AssignedToOrgId;
                    int UNASSIGNED = 1;
                    int ASSIGNED = 2;
                    int REROUTED = 8;

                    if((orgId == orgViewerIsFrom)  && (i==UNASSIGNED) || (i==ASSIGNED)  || (i == REROUTED))
                    {
                         retval = true;
                    }

                }
                catch (System.Exception)
                {
                }
                return retval;
            }
        }

        internal QmsCorrectiveactionrequest QmsCorrectiveActionRequest
        {
            get{
                QmsCorrectiveactionrequest r = new QmsCorrectiveactionrequest();
                r.ActionRequestTypeId = this.ActionRequestTypeId;
                r.AssignedAt = this.AssignedAt;
                r.AssignedByUserId = this.AssignedByUserId;
                r.AssignedToOrgId = this.AssignedToOrgId;
                r.AssignedToUserId = this.AssignedToUserId;
                r.CreatedAt = this.CreatedAt;
                r.CreatedByUserId = this.CreatedByUserId;
                r.DeletedAt = this.DeletedAt;
                r.EffectiveDateOfPar = this.EffectiveDateOfPar;
                r.EmplId = this.EmplId;
                r.Id = this.Id;
                r.CreatedAtOrgId = this.CreatedAtOrgId;
                r.NatureOfAction = this.NOACode;
                r.PareffectiveDate = this.ParEffectiveDate;
                r.ResolvedAt = this.ResolvedAt;
                r.StatusId = this.StatusId;
                r.SubmittedAt = this.SubmittedAt;
                r.UpdatedAt = this.UpdatedAt;
                r.Details = this.Details;
                r.IsPaymentMismatch = this.paymentMismatch;
                r.RowVersion = this.RowVersion;
                return r;
            }
        }


        internal CorrectiveAction(QmsCorrectiveactionrequest car) 
        {
            this.ActionId = 0;
            this.ActionRequestTypeId = car.ActionRequestTypeId;
            this.AssignedAt = car.AssignedAt;
            this.CreatedAtOrgId = car.CreatedAtOrgId.Value;
            this.CreatedByOrg = new Organization(car.CreatedAtOrg);
            this.AssignedToUserId = car.AssignedToUserId;
            if(this.AssignedToUserId.HasValue)
                this.AssignedToUser = new User(car.AssignedToUser,false,false);
            
            this.AssignedByUserId = car.AssignedByUserId;
            this.AssignedToOrgId = car.AssignedToOrgId;
            if(this.AssignedToOrgId.HasValue)
                this.AssignedToOrg = new Organization(car.AssignedToOrg);
            this.CreatedAt = car.CreatedAt;
            this.CreatedByUserId = car.CreatedByUserId;
            this.CreatedByUser = new User(car.CreatedByUser,false,false);
            this.DeletedAt = car.DeletedAt;
            this.EffectiveDateOfPar = car.EffectiveDateOfPar;
            this.EmplId = car.EmplId;
            this.Employee = new Employee(car.Empl);
            this.Id = car.Id;
            this.paymentMismatch = car.IsPaymentMismatch;
            this.NOACode = car.NatureOfAction;
            this.ParEffectiveDate = car.PareffectiveDate;
            this.ResolvedAt = car.ResolvedAt;
            this.StatusId = car.StatusId;
            this.Status = new Status(car.Status);
            this.SubmittedAt = car.SubmittedAt;
            this.UpdatedAt = car.UpdatedAt;
            this.Details = car.Details;
            this.Status = new Status(car.Status);
            this.ErrorTypes = new List<ErrorType>();
            this.RowVersion = car.RowVersion;
            foreach(var item in car.QmsCorrectiveactionErrortype)
            {
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.Id)................: {item.Id}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.CorrectiveActionId): {item.CorrectiveActionId}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.ErrorTypeId).......: {item.ErrorTypeId}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype..ErrorType == null): {item.ErrorType == null}");

                this.ErrorTypes.Add(new ErrorType(item));
            }


        }        

        public CorrectiveAction(QmsCorrectiveactionrequest car, User viewer) 
        {
            orgViewerIsFrom = viewer.OrgId.Value;
            this.ActionId = 0;
            this.ActionRequestTypeId = car.ActionRequestTypeId;
            this.AssignedAt = car.AssignedAt;
            this.CreatedAtOrgId = car.CreatedAtOrgId.Value;
            this.CreatedByOrg = new Organization(car.CreatedAtOrg);
            this.AssignedToUserId = car.AssignedToUserId;
            if(this.AssignedToUserId.HasValue)
                this.AssignedToUser = new User(car.AssignedToUser,false,false);
            
            this.AssignedByUserId = car.AssignedByUserId;
            this.AssignedToOrgId = car.AssignedToOrgId;
            if(this.AssignedToOrgId.HasValue)
                this.AssignedToOrg = new Organization(car.AssignedToOrg);
            this.CreatedAt = car.CreatedAt;
            this.CreatedByUserId = car.CreatedByUserId;
            this.CreatedByUser = new User(car.CreatedByUser,false,false); 
            this.DeletedAt = car.DeletedAt;
            this.EffectiveDateOfPar = car.EffectiveDateOfPar;
            this.EmplId = car.EmplId;
            this.Employee = new Employee(car.Empl);
            this.Id = car.Id;
            this.paymentMismatch = car.IsPaymentMismatch;
            this.NOACode = car.NatureOfAction;
            this.ParEffectiveDate = car.PareffectiveDate;
            this.ResolvedAt = car.ResolvedAt;
            this.StatusId = car.StatusId;
            this.Status = new Status(car.Status);
            this.SubmittedAt = car.SubmittedAt;
            this.UpdatedAt = car.UpdatedAt;
            this.Details = car.Details;
            this.ErrorTypes = new List<ErrorType>();
            this.RowVersion = car.RowVersion;
            foreach(var item in car.QmsCorrectiveactionErrortype)
            {
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.Id)................: {item.Id}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.CorrectiveActionId): {item.CorrectiveActionId}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype.ErrorTypeId).......: {item.ErrorTypeId}");
                Console.WriteLine($"[CorrectiveAction] => (QmsCorrectiveactionErrortype..ErrorType == null): {item.ErrorType == null}");

                this.ErrorTypes.Add(new ErrorType(item));
            }
            setReadOnly(viewer);

        }


        public CorrectiveAction()
        {
            paymentMismatch = "U";
            this.ActionId = 0;
            Comments = new List<CorrectiveActionComment>();
            ErrorTypes = new List<ErrorType>();
            Id = 0;
            RowVersion = 0;
        }

        internal CorrectiveActionListItem CorrectiveActionListItem()
        {
            CorrectiveActionListItem correctiveActionListItem = new CorrectiveActionListItem();
            correctiveActionListItem.Id = this.Id;
            correctiveActionListItem.EmplId = this.EmplId;
            correctiveActionListItem.EmployeeName = this.Employee.FullName;

            correctiveActionListItem.RequestType = this.ActionType.Label;

            correctiveActionListItem.NatureOfAction = this.NOACode + " - " + this.NatureOfAction.ShortDescription;
            if(this.AssignedToOrgId.HasValue)
                correctiveActionListItem.OrgAssigned = this.AssignedToOrg.Label;

            if (this.AssignedToUserId.HasValue)
                correctiveActionListItem.PersonAssigned = this.AssignedToUser.DisplayName;

            correctiveActionListItem.Status = this.Status.StatusLabel;
            correctiveActionListItem.PriorityIndex = this.PriorityIndex;
            correctiveActionListItem.Priority = this.Priority;
            correctiveActionListItem.SubmittedBy = this.CreatedByUser.DisplayName;
            correctiveActionListItem.DateSubmitted = this.CreatedAt;
            correctiveActionListItem.DaysOld = this.DaysSinceCreated;


            return correctiveActionListItem;
        }


    }
}