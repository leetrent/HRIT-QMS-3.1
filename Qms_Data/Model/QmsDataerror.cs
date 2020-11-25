using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsDataerror
    {
        public int DataErrorId { get; set; }
        public string DataErrorKey { get; set; }
        public int ErrorListId { get; set; }
        public string Emplid { get; set; }
        public string QmsErrorCode { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? AssignedByUserId { get; set; }
        public int? AssignedToOrgId { get; set; }
        public int? CreatedByUserId { get; set; }
        public int CreatedByOrgId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public string Details { get; set; }
        public int? StatusId { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string QmsErrorMessageText { get; set; }
        public int? CorrectiveActionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public byte? RowVersion { get; set; }

        public SecUser AssignedByUser { get; set; }
        public SecOrg AssignedToOrg { get; set; }
        public SecUser AssignedToUser { get; set; }
        public QmsCorrectiveactionrequest CorrectiveAction { get; set; }
        public SecOrg CreatedByOrg { get; set; }
        public SecUser CreatedByUser { get; set; }
        public QmsEmployee Empl { get; set; }
        public QmsMasterErrorList ErrorList { get; set; }
        public QmsStatus Status { get; set; }
    }
}
