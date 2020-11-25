using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        internal QMSContext context;

        public EmployeeRepository()
        {
            context = new QMSContext();
        }

        public EmployeeRepository(QMSContext qmsContext)
        {
            context = qmsContext;
        }


        public IQueryable<QmsEmployee> RetrieveAll()
        {
           return context.QmsEmployee.AsNoTracking(); 
        }

        public IQueryable<QmsEmployee> RetrieveAllActive()
        {
            return context.QmsEmployee.Where(e => e.DeletedAt == null).AsNoTracking();
        }

        public QmsEmployee RetrieveById(string employeeId)
        {
            return context.QmsEmployee.Where(e => e.EmplId == employeeId).SingleOrDefault();
        }
        public QmsEmployee RetrieveByEmailAddress(string emailAddress)
        {
            return context.QmsEmployee.Where(e => e.EmailAddress == emailAddress).SingleOrDefault();
        }
        
    }//end class
}//end namespace