namespace QmsCore.Services
{
    public interface IEmailService
    {
        void SendEmail(string sender, string recipient, string subject, string body);
        void SendEmail(string sender, string[] recipients, string subject, string body);
    }
}