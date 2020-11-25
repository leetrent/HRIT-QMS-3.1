using System;
using System.Linq;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public interface IEmployeeRepository
    {
        IQueryable<QmsEmployee> RetrieveAll();
        IQueryable<QmsEmployee> RetrieveAllActive();
        QmsEmployee RetrieveById(string employeeId);

        QmsEmployee RetrieveByEmailAddress(string emailAddress);
    }
}