using QmsCore.Model;
using System;
using System.Linq;

namespace QmsCore.Repository
{
    public interface IReferenceRepository
    {
       IQueryable<QmsNatureofaction> RetrieveNatureOfActions();  
        IQueryable<QmsErrortype> RetrieveErrorTypes();
        IQueryable<QmsCorrectiveactiontype> RetrieveActionTypes();


    }
}