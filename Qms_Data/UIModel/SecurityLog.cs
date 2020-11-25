using System;

namespace QmsCore.UIModel
{
    public class SecurityLog
    {
        public int SecurityLogId { get; set; }
        public int SecurityLogTypeId { get; set; }
        public int ActionTakenByUserId { get; set; }
        public int ActiontakenOnItemId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public User ActionTakenByUser { get; set; }
        public string SecurityLogType { get; set; }

    }//end class

}//end namespace