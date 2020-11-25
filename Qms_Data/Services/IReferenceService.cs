using System.Linq;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IReferenceService
    {
        IQueryable<NatureOfAction> RetrieveNatureOfActions();
        NatureOfAction RetrieveNatureOfAction(string noaCode);
        IQueryable<ErrorType> RetrieveErrorTypes();
        IQueryable<ActionType> RetrieveActionTypes();
        StatusTransition RetrieveStatusTransition(int qmsOrgStatusTransId);
        List<Status> RetrieveAvailableActionsList(string statusType, Organization org);
        List<Status> RetrieveAvailableActionsList(int statusId, Organization org);
        List<Status> RetrieveAvailableActionsList(int statusId, Organization org,string workItemType);
        SecOrg RetrieveOrgByOrgCode(string orgCode);
        List<Organization> RetrieveServiceCenters();
        Status RetrieveStatusByStatusCode(string statusCode);
        EmailLog RetrieveEmailLogByDate(string logDate);
        int SaveEmailLog(EmailLog emailLog);

        
    }
}
