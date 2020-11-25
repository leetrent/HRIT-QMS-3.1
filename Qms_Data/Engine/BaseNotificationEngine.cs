using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QmsCore.Model;
using QmsCore.QmsException;
using QmsCore.Repository;
using QmsCore.UIModel;
using QmsCore.Services;

namespace QmsCore.Engine
{
    internal class BaseNotificationEngine
    {
        internal QMSContext context;
        internal UserRepository userRepository;
        internal NotificationRepository notificationRepository;
        internal NtfNotificationevent notificationEvent;
        internal NtfNotificationeventtype notificationEventType;
        internal EmailService emailService;
        internal string message;
        internal string template;
        internal SecUser originator;
        internal BaseNotificationEngine()
        {
            context = new QMSContext();
            notificationRepository = new NotificationRepository(context);
            userRepository = new UserRepository(context);     
            emailService = new EmailService();       
        }

        internal BaseNotificationEngine(QMSContext qMSContext)
        {
            context = qMSContext;
            notificationRepository = new NotificationRepository(context);
            userRepository = new UserRepository(context);           
            emailService = new EmailService();       
        }        


        protected void send(string recepient,string subject, string body)
        {
            try
            {
                emailService.SendEmail("no-reply@gsa.gov",recepient,subject,body);                
            }
            catch (System.Exception)
            {
            }

        }

        protected void send(string[] recepient,string subject, string body)
        {

            try
            {
                emailService.SendEmail("no-reply@gsa.gov",recepient,subject,body);            }
            catch (System.Exception)
            {
            }            
        }

        internal string getActionRequestType(int actionRequestId)
        {
            if(actionRequestId == 1)
            {
                return "Correction Action";
            }
            else if(actionRequestId == 2)
            {
                return "Cancellation of Action";
            }
            else
            {
                return "Retro Action";
            }
        }

        internal List<SecUser> getReviewersInOrg(int orgId)
        {
            return userRepository.RetrieveAllReviewersByOrganizationId(orgId).ToList();
        }

        internal List<SecUser> getAllUsers()
        {
            return userRepository.RetrieveAllActiveUsers().ToList();
        }

    }//end class
}//end namespace