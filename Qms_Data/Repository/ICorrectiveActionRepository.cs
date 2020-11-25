using System;
using System.Linq;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public interface ICorrectiveActionRepository
    {
        IQueryable<QmsCorrectiveactionrequest> RetrieveAllByAssignedToUser(int AssignedToUserid);
        IQueryable<QmsCorrectiveactionrequest> RetrieveAllByAssignedToOrg(int AssignedToOrgId);
        IQueryable<QmsCorrectiveactionrequest> RetrieveAllByCreatedBy(int CreatedByUserId);
        IQueryable<QmsCorrectiveactionrequest> RetrieveAll();
        
    }
}