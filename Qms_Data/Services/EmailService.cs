using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;
using System.Net.Mail;

namespace QmsCore.Services
{
    public class EmailService : IEmailService
    {

        public void SendEmail(string sender, string recipient, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.To.Add(recipient);
            message.Subject = subject;
            message.From = new MailAddress(sender);
            message.Body = body;
            message.IsBodyHtml = true;
            send(message);
        }

        public void SendEmail(string sender, string[] recipients, string subject, string body)
        {
            MailMessage message = new MailMessage();
            foreach(string recipient in recipients)
            {
                message.To.Add(recipient);
            }
            message.Subject = subject;
            message.From = new MailAddress(sender);
            message.Body = body;
            message.IsBodyHtml = true;
            send(message);
        }

        private void send(MailMessage message)
        {
            NotificationService notificationService = new NotificationService();
            notificationService.SendEmail(message);
        }

    }
}