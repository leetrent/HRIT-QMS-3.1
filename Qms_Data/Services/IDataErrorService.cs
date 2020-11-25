using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Engine;
using QmsCore.QmsException;
using System.Text;

namespace QmsCore.Services
{
    public interface IDataErrorService
    {
        void SaveComment(string message,int entityId,int authorId);
        int? Save(int errorId, string details, int actionId, User submitter, int? assigneeId);
        DataError RetrieveById(int id, User searcher);
        List<DataErrorComment> RetrieveComments(int ehriErrorId);
        List<DataErrorListItem> RetrieveAllByEmployee(string employeeId);        
        List<DataErrorListItem> RetrieveAllForUser(User user);
        List<DataErrorListItem> RetrieveAllForUserArchive(User user);
        List<DataErrorListItem> RetrieveAll();
        List<DataErrorListItem> RetrieveAllArchive();
        List<DataErrorListItem> RetrieveAllByOrganization(User user);
        List<DataErrorListItem> RetrieveAllByOrganizationArchive(User user);

//        List<DataErrorListItem> RetrieveAllByEmployeePOID(string personnelOfficerIdentifier);
//        List<DataErrorListItem> RetrieveAllByEmployeePOIDArchive(string personnelOfficerIdentifier);

    }//end interface
}//end namespace