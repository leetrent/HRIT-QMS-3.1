using System.Linq;
using System.Collections.Generic;
using QmsCore.UIModel;
using Qms_Data.UIModel;

namespace QmsCore.Services
{
    public interface ICorrectiveActionService
    {
        void SaveComment(string message, int correctiveActionId, int authorId);
        int Save(CorrectiveAction newVersion, User submitter);
        List<CorrectiveActionComment> RetrieveComments(int correctiveActionId);
        List<CorrectiveActionHistory> RetrieveHistory(int correctiveActionId);
        CorrectiveAction RetrieveById(int Id, User searcher);
        List<CorrectiveActionListItem> RetrieveAllForUser(User user);
        List<CorrectiveActionListItem> RetrieveAllForUserArchive(User user);
        List<CorrectiveActionListItem> RetrieveAllForOrganization(User user);
        List<CorrectiveActionListItem> RetrieveAllForOrganizationArchive(User user);
        List<CorrectiveActionListItem> RetrieveAll();
        List<CorrectiveActionListItem> RetrieveAllArchive();
        List<CorrectiveActionListItem> RetrieveAllByEmployeePOID(User user);
        List<CorrectiveActionListItem> RetrieveAllByEmployeePOIDArchive(User user);
        List<CorrectiveActionListItem> RetrieveAgingReport(User user);

    }
}
