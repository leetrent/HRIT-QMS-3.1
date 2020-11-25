using System.Linq;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IEmployeeService
    {
        IQueryable<Employee> RetrieveAll();
        IQueryable<Employee> RetrieveAllActive();
        Employee RetrieveByEmailAddress(string emailAddress);
        Employee RetrieveById(string emplid);
    }
}