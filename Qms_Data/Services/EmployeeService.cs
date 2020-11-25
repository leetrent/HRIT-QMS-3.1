using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.QmsException;

namespace QmsCore.Services
{
    public class EmployeeService : IEmployeeService
    {
        EmployeeRepository repository;

        public EmployeeService()
        {
            repository = new EmployeeRepository();
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            repository = (EmployeeRepository)employeeRepository;
        }

        public IQueryable<Employee> RetrieveAll()
        {
           var retval = from recs in repository.RetrieveAll()
                        orderby recs.LastName, recs.FirstName, recs.MiddleName
                        select new Employee{
                            EmplId = recs.EmplId,
                            FirstName = recs.FirstName,
                            LastName = recs.LastName,
                            MiddleName = recs.MiddleName,
                            EmailAddress = recs.EmailAddress,
                            AgencySubElement = recs.AgencySubElement,
                            PersonnelOfficeIdentifier = recs.PersonnelOfficeIdentifier,
                            DepartmentId = recs.DepartmentId
                        };
            return retval;                        
        }

        public IQueryable<Employee> RetrieveAllActive()
        {
           var retval = from recs in repository.RetrieveAllActive()
                        where recs.DeletedAt == null
                        orderby recs.LastName, recs.FirstName, recs.MiddleName
                        select new Employee{
                            EmplId = recs.EmplId,
                            FirstName = recs.FirstName,
                            LastName = recs.LastName,
                            MiddleName = recs.MiddleName,
                            EmailAddress = recs.EmailAddress,
                            AgencySubElement = recs.AgencySubElement,
                            PersonnelOfficeIdentifier = recs.PersonnelOfficeIdentifier,
                            DepartmentId = recs.DepartmentId
                        };
            return retval;                        
        }

        public Employee RetrieveByEmailAddress(string emailAddress)
        {
            var qmsEmployee = repository.RetrieveByEmailAddress(emailAddress);
            if(qmsEmployee != null)
            {
                Employee uiEmployee = new Employee(qmsEmployee);
                return uiEmployee;
            }
            else
            {
                throw new EmployeeNotFoundException(string.Format("Employee with email '{0}' not found", emailAddress));
            }
        }

        public Employee RetrieveById(string emplid)
        {
            var qmsEmployee = repository.RetrieveById(emplid);
            if(qmsEmployee != null)
            {
                Employee uiEmployee = new Employee(qmsEmployee);
                return uiEmployee;
            }
            else
            {
                throw new EmployeeNotFoundException(string.Format("Employee with emplid '{0}' not found", emplid));                
            }
        }
    }
}