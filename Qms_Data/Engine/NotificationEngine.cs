using System;
using System.Collections.Generic;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QmsCore.Engine
{
    public class NotificationEngine
    {
        QMSContext context;
        private UserRepository userRepository;
        private NotificationRepository notificationRepository;
        private NtfNotificationevent notificationEvent;
        private NtfNotificationeventtype notificationEventType;

        private CorrectiveActionRepository correctiveActionRepository;

        private SecUser originator;


        public NotificationEngine()
        {
            context = new QMSContext();
            notificationRepository = new NotificationRepository(context);
            userRepository = new UserRepository(context);
            correctiveActionRepository = new CorrectiveActionRepository(context);
        }

        public NotificationEngine(QMSContext qMSContext)
        {
            context = qMSContext;
            notificationRepository = new NotificationRepository(context);
            userRepository = new UserRepository(context);
            correctiveActionRepository = new CorrectiveActionRepository(context);
        }        


        public void Send(IListable entity,string notificationEventCode, User submitter)
        {
            EmployeeService employeeService = new EmployeeService();
            Employee employee = employeeService.RetrieveById(entity.EmplId);
            notificationEvent = notificationRepository.RetrieveNotificationEventByCode(notificationEventCode);
            originator = userRepository.RetrieveByUserId(entity.CreatedByUserId.Value);
            bool willSendtoSubmitter = ((originator.UserId == submitter.UserId) && (notificationEvent.NotificationEventType.NotificationEventTypeCode == NotificationEventType.INDIV));
            if(!willSendtoSubmitter)
            {
                notificationEventType = notificationEvent.NotificationEventType;
                QmsWorkitemcomment comment = correctiveActionRepository.RetrieveLatestComment(entity.Id);
                switch(notificationEventType.NotificationEventTypeCode)
                {
                    case NotificationEventType.INDIV:
                        sendIndividualMessage(entity,notificationEvent, submitter,comment);
                        break;
                    case NotificationEventType.ORG:
                        sendOrganizationalMessage(entity,notificationEvent,comment);
                        break;
                    default:
                        break;
                }
            }
        }        
        private string getActionRequestType(int actionRequestId)
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

        private string formatMessage(IListable ca, QmsWorkitemcomment comment)
        {
            string retval = ca.Message;
            string commentTemplate = "Latest Comment:{0}<br/> by {1} on {2}";            
            if(comment != null)
            {
                string commentText = string.Format(commentTemplate,comment.Message,comment.Author.DisplayName,comment.CreatedAt.ToShortDateString());
                retval += commentText;
            }
            else
            {
                retval += "No comments have been made.";
            }

            return retval;
        }

        private void sendIndividualMessage(IListable entity, NtfNotificationevent ne, User submitter, QmsWorkitemcomment comment)
        {
            if(submitter.UserId != entity.CreatedByUserId.Value) // if the person doing the action is the originator they don't get a message since they did the action
            {
                NtfNotification notification = new NtfNotification();
                notification.CreatedAt = DateTime.Now;
                notification.HasBeenRead = 0;
                notification.Title = string.Format(ne.TitleTemplate,entity.Id);
                notification.WorkitemId = entity.Id;
                notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;
                notification.SendAsEmail = 1;
                notification.NotificationEventId = ne.NotificationEventId;
                notification.Message = entity.Message;
                switch(ne.NotificationEventCode)
                {
                    case CorrectiveActionNotificationType.CA_Assigned:
                        notification.UserId = entity.AssignedToUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.AssignedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Created:
                        notification.UserId = entity.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.CreatedAt.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Returned:
                        notification.UserId = entity.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.UpdatedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;
                    case CorrectiveActionNotificationType.CA_Closed:
                        notification.UserId = entity.CreatedByUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id, ca.ResolvedAt.Value.ToShortDateString(),ca.EmplId,ca.Employee.FullName);
                        break;                    
                    case CorrectiveActionNotificationType.CA_Withdrawn:
                        notification.UserId = entity.AssignedToUserId.Value;
//                        notification.Message = string.Format(ne.MessageTemplate,ca.Id,  ca.UpdatedAt.Value.ToShortDateString(), ca.EmplId, ca.Employee.FullName);
                        break;
                    default:
                        //not a indivual message type
                        break;
                }
        
                context.Add(notification);
                context.SaveChanges();
            }


            
        }

        private void sendOrganizationalMessage(IListable entity, NtfNotificationevent ne, QmsWorkitemcomment comment)
        {
            List<SecUser> users = getReviewersInOrg(entity.AssignedToOrgId.Value);
            NtfNotification notification = new NtfNotification();
            notification.CreatedAt = DateTime.Now;
            notification.Title = string.Format(ne.TitleTemplate,entity.Id);
            notification.Message = entity.Message;
            notification.HasBeenRead = 0;
            notification.WorkitemId = entity.Id;
            notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;
            notification.SendAsEmail = 1;
            notification.NotificationEventId = ne.NotificationEventId;

/*
            switch(ne.NotificationEventCode)
            {
                case CorrectiveActionNotificationType.CA_Submitted:
                    notification.Message = string.Format(ne.MessageTemplate,ca.Id, dateToUse,ca.EmplId,ca.Employee.FullName);
                    break;
                case CorrectiveActionNotificationType.CA_PendingReview:
                    notification.Message = string.Format(ne.MessageTemplate,ca.Id, dateToUse,ca.EmplId,ca.Employee.FullName);
                    break;
                case CorrectiveActionNotificationType.CA_Rerouted:
                    notification.Message = string.Format(ne.MessageTemplate,ca.Id, dateToUse,ca.EmplId,ca.Employee.FullName);
                    break;
                default:
                    break;
            }          
 */  

            foreach(var user in users)
            {
                NtfNotification newNotification = notification.Clone();
                newNotification.UserId = user.UserId;
                context.Add(newNotification);
            }

            context.SaveChanges();            
        }

        private List<SecUser> getReviewersInOrg(int orgId)
        {
            return userRepository.RetrieveAllReviewersByOrganizationId(orgId).ToList();
        }

        private List<SecUser> getAllUsers()
        {
            return userRepository.RetrieveAllActiveUsers().ToList();
        }



    }//end class
}//end namespace