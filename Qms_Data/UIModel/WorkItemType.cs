using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class WorkItemType
    {
        public string WorkItemTypeCode { get; set; }
        
        public int WorkItemId { get; set; }
        public string ControllerName { get; set; }
        public string MethodName { get; set; }

        public WorkItemType(QmsWorkitemtype qmsWorkItemType, int workItemId)
        {
            this.ControllerName = qmsWorkItemType.ControllerName;
            this.MethodName = qmsWorkItemType.MethodName;
            this.WorkItemTypeCode = qmsWorkItemType.WorkItemTypeCode;
            this.WorkItemId = workItemId;
        }
    }//end class
}//end namespace