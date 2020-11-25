
using System;
using System.Collections.Generic;
using QmsCore.Lib;

namespace QmsCore.Model
{
    public class UIMissingData : ITimeTrackable
    {
        public int Id { get; set; }
        public string EmplId { get; set; }
        public string MissingDataName { get; set; }
        public int? ErrorCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public int? StatusId { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? DeletedAt { get; set; }    
        public virtual QmsStatus Status { get; set; }  

        public int? CreatedByUserId { get; set; }

        public string WorkItemType {
            get {
                return Model.WorkItemTypeEnum.Recon;
            }
        }

       public int DaysSinceCreated {
            get{
                return DateCalc.DaysBetween(CreatedAt,DateTime.Now);
            }
        }

        public int? DaysSinceAssigned{
            get{
                if(AssignedAt.HasValue)
                {
                    return DateCalc.DaysBetween(AssignedAt.Value,DateTime.Now);
                }
                else{
                    return null;
                }
            }
        }

        public int? AssignedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? AssignedToOrgId { get; set; }

        public SecOrg AssignedToOrg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SecUser AssignedToUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SecUser AssignedByUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }//end class
}//end namespace