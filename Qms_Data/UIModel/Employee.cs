
using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Employee
    {
        public string EmplId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AgencySubElement { get; set; }
        public string PersonnelOfficeIdentifier { get; set; }

        public string PersonnelOfficeIdentifierDescription {get;set;}
        public string DepartmentId { get; set; }
        public string PayPlan { get; set; }
        public string Grade { get; set; }
        public string ManagerId { get; set;}         
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string UserKey { get; set; }    
        public string FullName
        {
            get{return this.LastName + ", " + this.FirstName;}
        }            
        public string SearchResultValue
        {
            get { return $"{this.LastName}, {this.FirstName} - [{this.EmplId}]"; }
        }

        public string DisplayName
        {
            get { return this.FirstName + " " + this.LastName; }
        }

        public Employee()
        {

        }

        public Employee(QmsEmployee employee)
        {
            this.EmplId = employee.EmplId;
            this.FirstName = employee.FirstName;
            this.MiddleName = employee.MiddleName;
            this.LastName = employee.LastName;
            this.EmailAddress = employee.EmailAddress;
            this.AgencySubElement = employee.AgencySubElement;
            this.PersonnelOfficeIdentifier = employee.PersonnelOfficeIdentifier;
            setPersonnelOfficeIdentifierDescription(employee.PersonnelOfficeIdentifier);
            this.DepartmentId = employee.DepartmentId;
            this.PayPlan = employee.PayPlan;
            this.Grade = employee.Grade;
            this.ManagerId = employee.ManagerId;
        }

        private void setPersonnelOfficeIdentifierDescription(string personnelOfficeIdentifier)
        {
            switch(personnelOfficeIdentifier)
            {
                case "4174":
                    PersonnelOfficeIdentifierDescription = "HR Services Center - PBS";
                    break;
                case "4019":
                    PersonnelOfficeIdentifierDescription = "HR Services Center - FAS";
                    break;
                case "4177":
                    PersonnelOfficeIdentifierDescription = "HR Services Center - Staff Offices";
                    break;
                case "1909":
                    PersonnelOfficeIdentifierDescription = "Executive Resources HR Service Center";
                    break;
                case "4250":
                    PersonnelOfficeIdentifierDescription = "Office Of Inspector General";
                    break;
                case "4008":
                    PersonnelOfficeIdentifierDescription = "CABS Service Center";
                    break;
                default:
                    PersonnelOfficeIdentifierDescription = "Unknown POI: " + personnelOfficeIdentifier;
                    break;
            }
        }
    }
}