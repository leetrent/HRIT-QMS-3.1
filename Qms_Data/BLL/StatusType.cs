namespace QmsCore.Model
{
    public class StatusType
    {
        public static readonly string UNASSIGNED = "UNASSIGNED";
        public static readonly string ASSIGNED = "ASSIGNED";
        public static readonly string PENDING_REVIEW = "PENDING_REVIEW";
        public static readonly string CLOSED = "CLOSED";
        public static readonly string CLOSED_ACTION_COMPLETED = "CLOSED_ACTION_COMPLETED";
        public static readonly string CLOSED_CONVERT_TO_CORR_ACTION = "CLOSED_CONVERT_TO_CORR_ACTION";
        public static readonly string RETURNED = "RETURNED";
        public static readonly string DRAFT = "DRAFT";
        public static readonly string NONE = "NONE";
        public static readonly string SAVE_AS_DRAFT = "SAVE_AS_DRAFT";
        public static readonly string SUBMIT_TO_REVIEWER = "SUBMIT_TO_REVIEWER";
        public static readonly string SUBMIT_FOR_RESOULTION = "SUBMIT_FOR_RESOULTION";
        public static readonly string REROUTED = "REROUTED";
    }
}