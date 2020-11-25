using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Qms_Data.UIModel
{
    public class CorrectiveActionListItem : IComparable<CorrectiveActionListItem>
    {
        public int Id { get; set; }
        public string EmplId { get; set; }
        public string EmployeeName { get; set; }
        public string RequestType { get; set; }
        public string NatureOfAction { get; set; }
        public string OrgAssigned { get; set; }
        public string PersonAssigned { get; set; }
        public string Status { get; set; }
        public double PriorityIndex { get; set; }
        public string Priority { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime DateSubmitted { get; set; }
        public int DaysOld { get; set; }

        public int CompareTo(CorrectiveActionListItem other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }
}
