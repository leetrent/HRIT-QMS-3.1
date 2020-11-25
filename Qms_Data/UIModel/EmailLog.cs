using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class EmailLog
    {
        public int EmailLogId { get; set; }
        public string SentDate { get; set; }
        public int SentAmount { get; set; }

        public EmailLog()
        {
            EmailLogId = 0;
        }


        public EmailLog(NtfEmaillog ntfEmailLog)
        {
            this.SentAmount = ntfEmailLog.SentAmount;
            this.SentDate = ntfEmailLog.SentDate;
            this.EmailLogId = ntfEmailLog.EmailLogId;
        }

        public NtfEmaillog NtfEmailLog()
        {
            NtfEmaillog log = new NtfEmaillog();
            log.SentAmount = this.SentAmount;
            log.SentDate = this.SentDate;
            log.EmailLogId = 0;
            return log;

        }

    }//end class
}//end namespace