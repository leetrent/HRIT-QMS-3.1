using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsEmployee
    {
        public QmsEmployee()
        {
            QmsCorrectiveactionrequest = new HashSet<QmsCorrectiveactionrequest>();
            QmsDataerror = new HashSet<QmsDataerror>();
        }

        public string EmplId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AgencySubElement { get; set; }
        public string PersonnelOfficeIdentifier { get; set; }
        public string DepartmentId { get; set; }
        public string PayPlan { get; set; }
        public string Grade { get; set; }
        public string ManagerId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string UserKey { get; set; }

        public ICollection<QmsCorrectiveactionrequest> QmsCorrectiveactionrequest { get; set; }
        public ICollection<QmsDataerror> QmsDataerror { get; set; }
    }
}
