using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using QmsCore.Lib;

namespace QmsCore.Model
{
    public partial class QmsCorrectiveactionrequest : IAssignable, ITimeTrackable
    {

        [NotMapped]
        public int DaysSinceCreated {
            get {
                return DateCalc.DaysBetween(this.CreatedAt, DateTime.Now);
            }
        }

        [NotMapped]
        public int? DaysSinceAssigned{
            get {
                return DateCalc.DaysBetween(this.AssignedAt.Value, DateTime.Now);
            }
        }

        [NotMapped]
        public string WorkItemType {
            get {
                return Model.WorkItemTypeEnum.CorrectiveActionRequest;
            }
        }


    }
}