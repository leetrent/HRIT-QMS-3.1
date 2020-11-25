using System;
using System.Text;
using System.Collections.Generic;
using QMS.ViewModels;
using QmsCore.UIModel;
using QmsCore.Services;

namespace QMS.Utils
{
    public static class CorrectiveActionUtil
    {
        public static CorrectiveAction MapToUIModelOnCreate(CorrectiveActionFormViewModel vm, string[] selectedErrorTypes)
        {
             var logSnippet = "[CorrectiveActionUtil][MapToUIModelOnCreate] => "; 

            CorrectiveAction newCorrectiveAction = new CorrectiveAction
            {
                Id                  = vm.CorrectiveActionId,
                ActionRequestTypeId = Int32.Parse(vm.ActionRequestTypeId),
                EmplId              = CorrectiveActionUtil.ExtractEmplId(vm.EmployeeSearchResult),  
                Employee            = null, // CorrectiveActionService will hydrate this object     
                NOACode             = vm.NatureOfAction,
                NatureOfAction      = new ReferenceService().RetrieveNatureOfAction(vm.NatureOfAction),
                EffectiveDateOfPar  = (DateTime)vm.EffectiveDateOfPar,   
                IsPaymentMismatch   = vm.IsPaymentMismatch,
                Details             = vm.Details,
                ActionId            = Int32.Parse(vm.StatusTypeId)
            };

            Console.WriteLine(logSnippet + $"(vm.CorrectiveActionId): '{vm.CorrectiveActionId}' / (vm.NatureOfAction): '{vm.NatureOfAction}'");
            Console.WriteLine(logSnippet + "(newCorrectiveAction.NatureOfAction == null): " + (newCorrectiveAction.NatureOfAction == null));
            if (newCorrectiveAction.NatureOfAction != null)
            {
                Console.WriteLine(logSnippet + $"(newCorrectiveAction.NatureOfAction.NoaCode): '{newCorrectiveAction.NatureOfAction.NoaCode}' / (newCorrectiveAction.NatureOfAction.RoutesToBr): '{newCorrectiveAction.NatureOfAction.RoutesToBr}'");
            }            

            ErrorTypeDictionary errorTypeDict = new ErrorTypeDictionary();
            foreach (var errorTypeId in selectedErrorTypes)
            {
                newCorrectiveAction.ErrorTypes.Add(errorTypeDict.GetErrorType(Int32.Parse(errorTypeId)));
            }

            return newCorrectiveAction;                  
        }

        public static CorrectiveAction MapToUIModelOnUpdate(CorrectiveActionFormViewModel vm, string[] selectedErrorTypes,  CorrectiveAction existingCA)
        {          
            var logSnippet = "[CorrectiveActionUtil][MapToUIModelOnUpdate] => "; 
            Console.WriteLine(logSnippet + $"(vm.Details): '{vm.Details}'");


            existingCA.EmplId              = CorrectiveActionUtil.ExtractEmplId(vm.EmployeeSearchResult);
            existingCA.Employee            = null; // CorrectiveActionService will hydrate this object
            existingCA.NOACode             = vm.NatureOfAction;
            existingCA.NatureOfAction      = new ReferenceService().RetrieveNatureOfAction(vm.NatureOfAction);
            existingCA.EffectiveDateOfPar  = (DateTime)vm.EffectiveDateOfPar;
            existingCA.IsPaymentMismatch   = vm.IsPaymentMismatch;
            existingCA.ActionId            = Int32.Parse(vm.StatusTypeId);
            existingCA.RowVersion          = vm.RowVersion;
            existingCA.ActionRequestTypeId = int.Parse(vm.ActionRequestTypeId);
            existingCA.Details             = vm.Details;

            if (vm.AssignedToUserId.HasValue)
            {
                existingCA.AssignedToUserId  = vm.AssignedToUserId;
            }         

            Console.WriteLine(logSnippet + $"(existingCA.Id): '{existingCA.Id}' / (existingCA.NatureOfAction): '{existingCA.NatureOfAction}'");
            Console.WriteLine(logSnippet + "(existingCA.NatureOfAction == null): " + (existingCA.NatureOfAction == null));
            if (existingCA.NatureOfAction != null)
            {
                Console.WriteLine(logSnippet + $"(existingCA.NatureOfAction.NoaCode): '{existingCA.NatureOfAction.NoaCode}' / (existingCA.NatureOfAction.RoutesToBr): '{existingCA.NatureOfAction.RoutesToBr}'");
            }
         
            ErrorTypeDictionary errorTypeDict = new ErrorTypeDictionary();
            existingCA.ErrorTypes = new List<ErrorType>();
            foreach (var errorTypeId in selectedErrorTypes)
            {
                existingCA.ErrorTypes.Add(errorTypeDict.GetErrorType(Int32.Parse(errorTypeId)));
            }

            return existingCA;           
        }
        public static CorrectiveActionFormViewModel MapToViewModel(CorrectiveAction entity, int userId, string useCase, List<ModuleMenuItem> moduleMenuItems, bool userCanAssign = false)
        {
            string logSnippet = "[CorrectiveActionUtil][MapToViewModel] => ";

            Console.WriteLine(logSnippet + $"(userId).................................: '{userId}'");
            Console.WriteLine(logSnippet + $"(useCase)................................: '{useCase}'");
            Console.WriteLine(logSnippet + $"(entity.IsReadOnly)......................: {entity.IsReadOnly}");
            Console.WriteLine(logSnippet + $"(userCanAssign)..........................: {userCanAssign}");
            Console.WriteLine(logSnippet + $"(CorrectiveAction.CreatedByUser == null).: {entity.CreatedByUser == null}");
            Console.WriteLine(logSnippet + $"(CorrectiveAction.CreatedByOrg == null)..: {entity.CreatedByOrg == null}");
            Console.WriteLine(logSnippet + $"(CorrectiveAction.AssignedToUser == null): {entity.AssignedToUser == null}");
            Console.WriteLine(logSnippet + $"(CorrectiveAction.AssignedToOrg == null).: {entity.AssignedToOrg == null}");

            CorrectiveActionFormViewModel viewModel = new CorrectiveActionFormViewModel{
                UserId                  = userId,
                CorrectiveActionId      = entity.Id,
                CorrectiveActionIdForAddComment = entity.Id,
                EmployeeSearchResult    = entity.Employee.SearchResultValue,
                NatureOfAction          = entity.NOACode,
                EffectiveDateOfPar      = entity.EffectiveDateOfPar,
                IsPaymentMismatch       = entity.IsPaymentMismatch,
                ActionRequestTypeId     = entity.ActionRequestTypeId.ToString(),
                Details                 = entity.Details,
                CanAssign               = userCanAssign && entity.IsAssignable,
                Comments                = entity.Comments,
                Histories               = entity.Histories,
                StatusLabel             = entity.Status.StatusLabel,
                DateSubmitted           = entity.CreatedAt.ToString("MMMM dd, yyyy"),
                CreatedByUserName       = entity.CreatedByUser.DisplayName,
                CreatedByOrgLabel       = entity.CreatedByOrg.OrgLabel,
                AssignedToUserName      = (entity.AssignedToUser == null) ? "Unassigned" : entity.AssignedToUser.DisplayName,
                AssignedToOrgLabel      = (entity.AssignedToOrg == null)  ? "Unassigned" : entity.AssignedToOrg.OrgLabel,
                RowVersion              = entity.RowVersion,
                PersonnelOfficeIDDesc   = entity.Employee.PersonnelOfficeIdentifierDescription,
                IsReadOnly              = entity.IsReadOnly,
                UseCase                 = useCase,
                Controller              = MenuUtil.FindControllerForUseCase(useCase, moduleMenuItems)
            };

            if ( String.IsNullOrEmpty(viewModel.Details)
                    || String.IsNullOrWhiteSpace(viewModel.Details))
            {
                viewModel.Details = "None provided";
            }
            
            if ( entity.StatusId != null && entity.StatusId.HasValue )
            {
                viewModel.CurrentStatusId = (int)entity.StatusId;
            }

            Console.WriteLine(logSnippet + "BEGIN: CorrectiveActionFormViewModel.ToString()");
            Console.WriteLine(viewModel);
            Console.WriteLine(logSnippet + "END: CorrectiveActionFormViewModel.ToString()");

            return viewModel;
        }
        public static string[] ExtractErrorTypeIds(CorrectiveAction entity)
        {
            List<string> errorTypeIdList = new List<string>();

            if ( entity != null && entity.ErrorTypes != null)
            {
                foreach (var errorType in entity.ErrorTypes)
                {
                    errorTypeIdList.Add(errorType.Id.ToString());
                }
            }

            return errorTypeIdList.ToArray();
        }
        public static String ExtractEmplId(string employeeSearchResult) 
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CorrectiveActionUtil][ExtractEmplId] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(employeeSearchResult): '{employeeSearchResult}'");

            string emplId = null;

            if ( employeeSearchResult != null )
            {
                int start  = employeeSearchResult.IndexOf('[') + 1;
                int end    = employeeSearchResult.IndexOf(']');
                int length = end - start;

                if ( start != -1 && end != -1 )
                {
                    emplId = employeeSearchResult.Substring(start, length);
                }
                else
                {
                    emplId = employeeSearchResult;
                }
            }

            Console.WriteLine(logSnippet + $"(returning): '{emplId}'");
            return emplId;
        }
    }
}