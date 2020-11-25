using System;

namespace QMS_NotificationSender
{
    public class SettingType
    {
        public static string SendEventBasedEmail = "SendEventBasedEmail";
        public static string SendTimeBasedEmail  = "SendTimeBasedEmail";
        public static string EmailServer = "EmailServer";
        public static string EmailFooter = "EmailFooter";
        public static string CA_INDRAFT_SPECIALIST = "CA_INDRAFT_SPECIALIST";
        public static string CA_INDRAFT_REVIEWER = "CA_INDRAFT_REVIEWER";
        public static string CA_RETURNED_SPECIALIST = "CA_RETURNED_SPECIALIST";
        public static string CA_RETURNED_REVIEWER = "CA_RETURNED_REVIEWER";
        public static string CA_ASSIGNED_SC_SPECIALIST = "CA_ASSIGNED_SC_SPECIALIST";
        public static string CA_ASSIGNED_PPRM_SPECIALIST = "CA_ASSIGNED_PPRM_SPECIALIST";
        public static string CA_ASSIGNED_BRC_SPECIALIST = "CA_ASSIGNED_BRC_SPECIALIST";
        public static string CA_ASSIGNED_SC_REVIEWER = "CA_ASSIGNED_SC_REVIEWER";
        public static string CA_ASSIGNED_PPRM_REVIEWER = "CA_ASSIGNED_PPRM_REVIEWER";
        public static string CA_ASSIGNED_BRC_REVIEWER = "CA_ASSIGNED_BRC_REVIEWER";
        public static string CA_PENDING_ASSIGNMENT = "CA_PENDING_ASSIGNMENT";

        public static string CA_PENDING_REVIEW = "CA_PENDING_REVIEW";
        public static string CA_URL = "CA_URL";        

        public static string FromEmail = "FromEmail";
    }//end class
}//end namespace