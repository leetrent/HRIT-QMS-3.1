using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.ViewModels
{
    public class CorrectiveActionListViewModel
    {
        [Display(Name = "ID")]
        public int CorrectiveActionId { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Request Type")]
        public string ActionTypeLabel { get; set; }

        [Display(Name = "Nature of Action")]
        public string NatureOfActionLabel { get; set; }

        [Display(Name = "Status Type")]
        public string StatusTypeLabel { get; set; }
        
        [Display(Name = "Org Assigned")]
        public string AssignedToOrgLabel{ get; set; }

        [Display(Name = "Person Assigned")]
        public string AssignedToUserName { get; set;}

        [Display(Name = "Status")]
        public string StatusLabel { get; set; }

        [Display(Name = "Priority")]
        public string Priority { get; set; }
        
        [Display(Name = "Submitted By")]
        public string CreatedByUserName { get; set; }

        [Display(Name = "Date Submitted")]
        public string CreatedAt { get; set; }

        [Display(Name = "Days Old")]
        public int DaysSinceCreated { get; set; }

        public string UseCase {get; set;}
    }
}