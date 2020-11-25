using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.ViewModels;
using QMS.Extensions;
using QMS.Constants;

namespace QMS.ViewComponents
{
	public class CAErrorCategoriesViewComponent : ViewComponent
	{	
        private readonly IReferenceService _referenceService;

        public CAErrorCategoriesViewComponent(IReferenceService refSvc)
        {
            _referenceService = refSvc;
        }

        public  IViewComponentResult Invoke()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The HttpContext.Items collection is used to store data while processing a single request.
            // The collection's contents are discarded after a request is processed.
            /////////////////////////////////////////////////////////////////////////////////////////////
            string[] selectedErrorTypeIdStrings = (string[])HttpContext.Items[CorrectiveActionsConstants.SELECTED_ERROR_TYPES_KEY];

            if ( selectedErrorTypeIdStrings == null )
            {
                selectedErrorTypeIdStrings = new string[0];
            }

            //////////////////////////////////////////////////////////////////////////////////////////////
            // Convert the passed-in string array of ids to an integer array of ids
            //////////////////////////////////////////////////////////////////////////////////////////////   
            int[] selectedErrorTypeIds = Array.ConvertAll(selectedErrorTypeIdStrings, int.Parse);
            var selectedErrorTypeIdSet = new HashSet<int>(selectedErrorTypeIds);

            //////////////////////////////////////////////////////////////////////////////////////////////
            // Retreive all error types
            //////////////////////////////////////////////////////////////////////////////////////////////  
            List<ErrorType> dbErrorTypes = new ReferenceService().RetrieveErrorTypes().ToList();

            //////////////////////////////////////////////////////////////////////////////////////////////
            // Create a View Model for each ErrorType amd place each in a View Model collection
            // If the IDs in both the ErrorType collection and array of IDs match,
            // assign the value of TRUE to the 'Selected' property.
            ////////////////////////////////////////////////////////////////////////////////////////////// 
            int[] errorTypeSortingIndices = new int[dbErrorTypes.Count];
            int count = 0;
            for (int column = 0; column < CorrectiveActionsConstants.NUMBER_OF_ERROR_TYPE_COLUMNS; column++)
            {
                for (int row = column; row < dbErrorTypes.Count; row += CorrectiveActionsConstants.NUMBER_OF_ERROR_TYPE_COLUMNS)
                {
                       errorTypeSortingIndices[count++] = row;
                }
            }

            ErrorTypeViewModel[] vmErrorTypes = new ErrorTypeViewModel[dbErrorTypes.Count];
            int index = 0;
            foreach (ErrorType dbErrorType in dbErrorTypes)
            {
                ErrorTypeViewModel vmErrorType = new ErrorTypeViewModel
                {
                    Id = (int)dbErrorType.Id,
                    Description = dbErrorType.Description,
                    Selected = selectedErrorTypeIdSet.Contains((int)dbErrorType.Id)               
                };
                vmErrorTypes[errorTypeSortingIndices[index++]] = vmErrorType;
            }
            ViewBag.ErrorTypes = new List<ErrorTypeViewModel>(vmErrorTypes);

            //////////////////////////////////////////////////////////////////////////////////////////////
            // Set SELECTED_ERROR_TYPES back to null
            ////////////////////////////////////////////////////////////////////////////////////////////// 
            HttpContext.Items[CorrectiveActionsConstants.SELECTED_ERROR_TYPES_KEY] = null;

            return View();
        }
	}
}