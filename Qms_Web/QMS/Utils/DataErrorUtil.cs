using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.UIModel;
using QMS.ViewModels;

namespace QMS.Utils
{
    public static class DataErrorUtil
    {
        public static DataErrorViewModel MapToViewModel(UserViewModel userVM, DataError svcDataError, string useCase, List<ModuleMenuItem> moduleMenuItems)
        { 
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][DataErrorUtil][MapToViewModel] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(svcDataError.Histories == null): {svcDataError.Histories == null}");
            Console.WriteLine(logSnippet + $"(svcDataError.Histories.Count)..: {svcDataError.Histories.Count}");

            DataErrorViewModel vmDataError = new DataErrorViewModel();

            vmDataError.UserId              = userVM.UserId;
            vmDataError.DataErrorId         = svcDataError.DataErrorId;
            vmDataError.DataErrorIdForAddComment = svcDataError.DataErrorId;
            vmDataError.CreatedAt           = svcDataError.CreatedAt;
            vmDataError.IsAssignable        = (userVM.CanAssignTasks && svcDataError.IsAssignable);
            vmDataError.AssignedToUser      = svcDataError.AssignedToUser;
            vmDataError.Status              = svcDataError.Status;
            vmDataError.CorrectiveActionId  = svcDataError.CorrectiveActionId;
            vmDataError.Employee            = svcDataError.Employee;
            vmDataError.QmsErrorCode        = svcDataError.QmsErrorCode;
            vmDataError.DataElement         = svcDataError.DataElement;
            vmDataError.QmsErrorMessageText = svcDataError.QmsErrorMessageText;
            vmDataError.Comments            = svcDataError.Comments;
            vmDataError.Histories           = svcDataError.Histories;
            vmDataError.IsReadOnly          = svcDataError.IsReadOnly;
            vmDataError.UseCase             = useCase;
            vmDataError.Controller          = MenuUtil.FindControllerForUseCase(useCase, moduleMenuItems);
            vmDataError.EmployeeName        = svcDataError.Employee.SearchResultValue;
            vmDataError.Details             = svcDataError.Details;

            return vmDataError;
        }
    }
}