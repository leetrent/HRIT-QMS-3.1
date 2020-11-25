using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsCorrectiveactionrequest
    {
        public QmsCorrectiveactionrequest()
        {
            QmsCorrectiveactionErrortype = new HashSet<QmsCorrectiveactionErrortype>();
            QmsDataerror = new HashSet<QmsDataerror>();
        }

        public int Id { get; set; }
        public int? ActionRequestTypeId { get; set; }
        public string EmplId { get; set; }
        public string NatureOfAction { get; set; }
        public DateTime EffectiveDateOfPar { get; set; }
        public string IsPaymentMismatch { get; set; }
        public DateTime? PareffectiveDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? AssignedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? AssignedToOrgId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public int? StatusId { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public string Details { get; set; }
        public int? CreatedAtOrgId { get; set; }
        public byte RowVersion { get; set; }

        public QmsCorrectiveactiontype ActionRequestType { get; set; }
        public SecUser AssignedByUser { get; set; }
        public SecOrg AssignedToOrg { get; set; }
        public SecUser AssignedToUser { get; set; }
        public SecOrg CreatedAtOrg { get; set; }
        public SecUser CreatedByUser { get; set; }
        public QmsEmployee Empl { get; set; }
        public QmsNatureofaction NatureOfActionNavigation { get; set; }
        public QmsStatus Status { get; set; }
        public ICollection<QmsCorrectiveactionErrortype> QmsCorrectiveactionErrortype { get; set; }
        public ICollection<QmsDataerror> QmsDataerror { get; set; }
    }
}
