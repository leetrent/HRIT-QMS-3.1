using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;

namespace QMS.ViewModels
{
    public class DataErrorViewModel
    {
        public DateTime CreatedAt { get; set; }
        public User AssignedToUser { get; set; }
        public Status Status { get; set; }
        public int? CorrectiveActionId { get; set; }

        public Employee Employee { get; set; }

        public string QmsErrorCode { get; set; }
        public string DataElement { get; set; }
        public string QmsErrorMessageText { get; set; }

        //[Required]
        //[Display(Name = "Action Type")]
        //public int StatusId { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // StatusType
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name = "Action Type")]
        public String StatusId { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // Error  Details
        ////////////////////////////////////////////////////////////////////////////////
        [Required]
        [Display(Name = "Error Details")]
        [MinLength(5, ErrorMessage = "At least 5 characters are required.")]
        [MaxLength(2000, ErrorMessage = "A maximum of 2,000 characters are permitted.")]
        public string Details { get; set; }

        [HiddenInput]
        public int DataErrorId { get; set; }

        [HiddenInput]
        public string UseCase { get; set; }

        [HiddenInput]
        public int UserId { get; set; }

        [HiddenInput]
        public int DataErrorIdForAddComment { get; set; }

        [HiddenInput]
        public string EmployeeName { get; set; }

        public string Controller { get; set; }

        public bool IsReadOnly { get; set; } = true;

        public bool IsAssignable { get; set; } = false;

        public int? AssignedToUserId { get; set; }

        public List<DataErrorComment> Comments { get; set; }

        public List<DataErrorHistory> Histories { get; set; }
    }
}