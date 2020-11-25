using System;
using System.Collections.Generic;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QmsCore.Engine
{
    internal class CorrectiveActionNotificationEngine : BaseNotificationEngine
    {


        private CorrectiveActionRepository correctiveActionRepository;
        private string footerTemplate = "https://hrqms.gsa.gov/Home/Warning?requestedUri=CorrectiveActions/Edit/{0}";

        public CorrectiveActionNotificationEngine() : base()
        {
            correctiveActionRepository = new CorrectiveActionRepository(context);
        }

        public CorrectiveActionNotificationEngine(QMSContext qMSContext) : base(qMSContext)
        {
            correctiveActionRepository = new CorrectiveActionRepository(context);
        }        

        public void Send(CorrectiveAction correctiveAction,string notificationEventCode, User submitter)
        {
            EmployeeService employeeService = new EmployeeService();
            correctiveAction.Employee = employeeService.RetrieveById(correctiveAction.EmplId);
            notificationEvent = notificationRepository.RetrieveNotificationEventByCode(notificationEventCode);
            originator = userRepository.RetrieveByUserId(correctiveAction.CreatedByUserId.Value);
            bool willSendtoSubmitter = ((originator.UserId == submitter.UserId) && (notificationEvent.NotificationEventType.NotificationEventTypeCode == NotificationEventType.INDIV));
            if(!willSendtoSubmitter)
            {
                notificationEventType = notificationEvent.NotificationEventType;
                QmsWorkitemcomment comment = correctiveActionRepository.RetrieveLatestComment(correctiveAction.Id);
                switch(notificationEventType.NotificationEventTypeCode)
                {
                    case NotificationEventType.INDIV:
                        sendIndividualMessage(correctiveAction,notificationEvent, submitter,comment);
                        break;
                    case NotificationEventType.ORG:
                        sendOrganizationalMessage(correctiveAction,notificationEvent,comment);
                        break;
                    default:
                        break;
                }
            }
        } 


//        internal string formatMessage(CorrectiveAction ca, QmsWorkitemcomment comment)
        internal string formatMessage(CorrectiveAction ca, QmsWorkitemcomment comment)
        {
            template = "Corrective Acton ID: {0}<br/>Employee: {1}-{2}<br/>Request Type: {3}<br/>NOA: {4}<br/>Effective Date: {5}<br/>Updated on: {6}<br/>Correction Requested:{7}<br/>";
            string dateToUse = ca.UpdatedAt.HasValue ? ca.UpdatedAt.Value.ToShortDateString() : ca.CreatedAt.ToShortDateString();
            message = string.Format(template,ca.Id,ca.EmplId, ca.Employee.FullName, getActionRequestType(ca.ActionRequestTypeId.Value) ,ca.NOACode,ca.EffectiveDateOfPar,dateToUse,ca.Details);
            string commentTemplate = "Latest Comment:{0}<br/> by {1} on {2}";
            if(comment != null)
            {
                string commentText = string.Format(commentTemplate,comment.Message,comment.Author.DisplayName,comment.CreatedAt.ToShortDateString());
                message += commentText;
            }
            else
            {
                message += "No comments have been made.";
            }
            message += "<br/><br/>" + string.Format(footerTemplate,ca.Id);
            return message;
        }

//        internal void sendIndividualMessage(CorrectiveAction ca, NtfNotificationevent ne, User submitter, QmsWorkitemcomment comment)

        internal void sendIndividualMessage(CorrectiveAction ca, NtfNotificationevent ne, User submitter, QmsWorkitemcomment comment)
        {
            if(submitter.UserId != ca.CreatedByUserId.Value) // if the person doing the action is the originator they don't get a message since they did the action
            {
                SecUser user;                
                NtfNotification notification = new NtfNotification();
                notification.CreatedAt = DateTime.Now;
                notification.HasBeenRead = 0;
                notification.Title = string.Format(ne.TitleTemplate,ca.Id);
                notification.WorkitemId = ca.Id;
                notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;
                notification.SendAsEmail = 1;
                notification.NotificationEventId = ne.NotificationEventId;
                notification.Message = formatMessage(ca,comment);
                switch(ne.NotificationEventCode)
                {
                    case CorrectiveActionNotificationType.CA_Assigned:
                        notification.UserId = ca.AssignedToUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.AssignedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Created:
                        notification.UserId = ca.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.CreatedAt.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Returned:
                        notification.UserId = ca.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.UpdatedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Closed:
                        notification.UserId = ca.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.ResolvedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;                    
                    case CorrectiveActionNotificationType.CA_Withdrawn:
                        notification.UserId = ca.AssignedToUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id,  ca.UpdatedAt.Value.ToShortDateString(), ca.EmplId, ca.Employee.FullName);
                        break;
                    default:
                        //not a indivual message type
                        break;
                }
                user = userRepository.RetrieveByUserId(notification.UserId);
                send(user.EmailAddress,notification.Title,notification.Message);

                context.Add(notification);
                context.SaveChanges();
            }
        }

//        internal void sendOrganizationalMessage(CorrectiveAction ca, NtfNotificationevent ne, QmsWorkitemcomment comment)

        internal void sendOrganizationalMessage(CorrectiveAction ca, NtfNotificationevent ne, QmsWorkitemcomment comment)
        {
            byte byteTrue = 1;
            byte byteFalse = 0;

            List<SecUser> users = getReviewersInOrg(ca.AssignedToOrgId.Value);
            string[] emails = new string[users.Count];
            NtfNotification notification = new NtfNotification();
            notification.CreatedAt = DateTime.Now;
            notification.Title = string.Format(ne.TitleTemplate,ca.Id);
            notification.Message = formatMessage(ca,comment);
            notification.SendAsEmail = byteTrue;
            notification.HasBeenRead = byteFalse;
            notification.WorkitemId = ca.Id;
            notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;

            notification.NotificationEventId = ne.NotificationEventId;

            int i = 0;
            foreach(var user in users)
            {
                NtfNotification newNotification = notification.Clone();
                newNotification.UserId = user.UserId;
                context.Add(newNotification);
                emails[i] = user.EmailAddress;
                i++;                
            }
            send(emails,notification.Title,notification.Message);

            var entries = context.ChangeTracker.Entries().Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry.Entity.ToString());
            }




            context.SaveChanges();            
        }





    }//end class
}//end namespace