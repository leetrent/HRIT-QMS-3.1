namespace QMS.ViewModels
{
    public class EmailViewModel
    {
        public string Sender { get; set; }
        public string Recipient { get; set; } = "qms.support@gsa.gov";
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool   ShowAlert { get; set; } = false;
    }
}