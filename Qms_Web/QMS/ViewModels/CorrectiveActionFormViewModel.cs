using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;

namespace QMS.ViewModels
{
    public class CorrectiveActionFormViewModel
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Employee Search
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name ="Employee")]
        [RegularExpression(@".*\d{8}.*", ErrorMessage = "Employee ID is required.")]
        
        public string EmployeeSearchResult { get; set; }
  
        public bool ShowInactiveEmployees { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // NatureOfAction drop-down
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name ="Nature of Action")]
        public string NatureOfAction { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // EffectiveDateOfPar date-picker
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [DataType(DataType.Date)]
        [Display(Name ="Effective PAR Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EffectiveDateOfPar { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // IsPaymentMismatch checkbox
        ////////////////////////////////////////////////////////////////////////////////
        public bool IsPaymentMismatch { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // ActionType
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name ="Request Type")]
        public String ActionRequestTypeId { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // Correction Details
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [MinLength(5, ErrorMessage="At least 5 characters are required.")]
        [MaxLength(2000, ErrorMessage="A maximum of 2,000 characters are permitted.")]
        public string Details { get; set; } 

        ////////////////////////////////////////////////////////////////////////////////
        // StatusType
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name ="Action Type")]
        public String StatusTypeId { get; set; }

        [HiddenInput]
        public int CorrectiveActionId { get; set; }

        [HiddenInput]
        public int CorrectiveActionIdForAddComment { get; set; }

        [HiddenInput]
        public int CurrentStatusId { get; set; }

        [HiddenInput]
        public byte RowVersion { get; set; }

        [HiddenInput]
        public int UserId { get; set; }

        public bool CanAssign { get; set; } = false;

        public int? AssignedToUserId { get; set; }

        public string Comment { get; set; }
        
        public List<CorrectiveActionComment> Comments {get;set;}

        public List<CorrectiveActionHistory> Histories {get;set;}

        public string CreatedByUserName { get; set; }

        public string CreatedByOrgLabel { get; set; }

        public string AssignedToUserName { get; set; }

        public string AssignedToOrgLabel { get; set; }    

        public string StatusLabel { get; set; }

        public string DateSubmitted { get; set; }

        public string PersonnelOfficeIDDesc { get; set; }

        public bool IsReadOnly {get; set; } = true;

        public string UseCase {get; set;}

        public string Controller {get; set;}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CorrectiveActionFormViewModel = {");
            sb.Append("UserId: ");
            sb.Append(this.UserId);
            sb.Append(", CorrectiveActionId: ");
            sb.Append(this.CorrectiveActionId);
            sb.Append(", CanAssign: ");
            sb.Append(this.CanAssign);
            sb.Append(", EmployeeSearchResult: ");
            sb.Append(this.EmployeeSearchResult);
            sb.Append(", ShowInactiveEmployees: ");
            sb.Append(this.ShowInactiveEmployees);
            sb.Append(", NatureOfAction: ");
            sb.Append(this.NatureOfAction);
            sb.Append(", EffectiveDateOfPar: ");
            sb.Append(this.EffectiveDateOfPar);
            sb.Append(", IsPaymentMismatch: ");
            sb.Append(this.IsPaymentMismatch);
            sb.Append(", ActionRequestTypeId: ");
            sb.Append(this.ActionRequestTypeId);
            sb.Append(", StatusTypeId: ");
            sb.Append(this.StatusTypeId);
            sb.Append(", CorrectiveActionId: ");
            sb.Append(this.CorrectiveActionId);
            sb.Append(", CurrentStatusId: ");
            sb.Append(this.CurrentStatusId);
            sb.Append(", CanAssign: ");
            sb.Append(this.CanAssign);
            sb.Append(", AssignedToUserId: ");
            sb.Append(this.AssignedToUserId);  
            sb.Append(", Comment: ");
            sb.Append(this.Comment); 
            sb.Append(", CreatedByUserName: ");
            sb.Append(this.CreatedByUserName); 
            sb.Append(", CreatedByOrgLabel: ");
            sb.Append(this.CreatedByOrgLabel); 
            sb.Append(", AssignedToUserName: ");
            sb.Append(this.AssignedToUserName); 
            sb.Append(", AssignedToOrgLabel: ");
            sb.Append(this.AssignedToOrgLabel);
            sb.Append(", StatusLabel: ");
            sb.Append(this.StatusLabel); 
            sb.Append(", DateSubmitted: ");
            sb.Append(this.DateSubmitted); 
            sb.Append(", RowVersion: ");
            sb.Append(this.RowVersion); 
            sb.Append(", PersonnelOfficeIDDesc: ");
            sb.Append(this.PersonnelOfficeIDDesc); 
            sb.Append(", IsReadOnly: ");
            sb.Append(this.IsReadOnly); 
            sb.Append("}");

            return sb.ToString();
        }
    }
}