
using System;
using System.Collections.Generic;
using QmsCore.Lib;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class DataErrorListItem : IComparable<DataErrorListItem>
    {

        public int EhriErrorId { get; set; }
        public string EmplId {get;set;}
        public string OfficeSymbol { get; set; }
        public string EmployeeName { get; set; }
        public string ErrorCode { get; set; }
        public string Category { get; set; }
        public string DataElement { get; set; }       
        public string Status { get; set; }
        public string AssignedTo {get;set;}
        public DateTime DateCreated { get; set; }
        public int DaysOpen { get; set; }
        public int PriorityIndex { get; set; }
        public string Priority { get; set; }
        public int? CorrectiveActionId { get; set; }

        ////////////////////////////////////
        // TEMPORARY (Lee on Sept 14):
        ////////////////////////////////////
        public string UseCase { get; set; }


        public DataErrorListItem()
        {}

        public int CompareTo(DataErrorListItem other)
        {
            return this.EhriErrorId.CompareTo(other.EhriErrorId);
        }


    }//end class
}//end namespace