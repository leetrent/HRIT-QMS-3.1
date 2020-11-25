using System;
using System.Collections.Generic;
using  QmsCore.UIModel;

namespace QmsCore.Model
{
    public interface IAssignable
    {
        int Id {get;set;}
        DateTime CreatedAt { get; set; }
        int? AssignedByUserId { get; set; }
        int? AssignedToUserId { get; set; }
        int? AssignedToOrgId { get; set; }
        DateTime? AssignedAt { get; set; }
        int? StatusId { get; set; }
        DateTime? SubmittedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? ResolvedAt { get; set; }
        DateTime? DeletedAt { get; set; }
  
        string WorkItemType { get; }    
        int? CreatedByUserId { get; set; }   
    }
}