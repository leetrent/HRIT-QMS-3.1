using System;
using QmsCore.Model;


namespace QmsCore.UIModel
{
    public interface IListable : IAssignable, IMessageCreatable
    {
        User AssignedByUser {get;set;}
        User CreatedByUser {get;set;}
        User AssignedToUser {get;set;}
        Status Status {get;set;}
        Organization CreatedByOrg {get;set;}
        Organization AssignedToOrg {get;set;}
        Employee Employee { get; set; }

        string EmplId {get;set;}
    }//end interface
}//end namespace