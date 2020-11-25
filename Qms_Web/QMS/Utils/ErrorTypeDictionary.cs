using System;
using System.Linq;
using System.Collections.Generic;
using QmsCore.UIModel;
using QmsCore.Services;

namespace QMS.Utils
{
    public class ErrorTypeDictionary
    {
        private Dictionary<int?, ErrorType> _errorTypes = new Dictionary<int?, ErrorType>();

        public ErrorTypeDictionary()
        {
            IQueryable<ErrorType> errorTypes = new ReferenceService().RetrieveErrorTypes();
            foreach ( var errorType in errorTypes)
            {
                _errorTypes.Add(errorType.Id, errorType);
            }
        }

        public ErrorType GetErrorType(int? errorTypeId)
        {
            return _errorTypes[errorTypeId];
        }
    }
}