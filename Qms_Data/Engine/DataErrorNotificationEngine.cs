using System;
using System.Collections.Generic;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.Services;
using QmsCore.UIModel;

namespace QmsCore.Engine
{
    internal class DataErrorNotificationEngine: BaseNotificationEngine
    {
        private DataErrorRepository ehriErrorRepository;



        public DataErrorNotificationEngine() : base()
        {
            ehriErrorRepository = new DataErrorRepository(context);
        }

        public DataErrorNotificationEngine(QMSContext qmsContext) : base(qmsContext)
        {
            ehriErrorRepository = new DataErrorRepository(context);

        }


        public void Send(DataError ehriError,string notificationEventCode, User submitter)
        {
            EmployeeService employeeService = new EmployeeService();
            ehriError.Employee = employeeService.RetrieveById(ehriError.EmplId);
            notificationEvent = notificationRepository.RetrieveNotificationEventByCode(notificationEventCode);
            originator = userRepository.RetrieveByUserId(ehriError.CreatedByUserId.Value);
            bool willSendtoSubmitter = ((originator.UserId == submitter.UserId) && (notificationEvent.NotificationEventType.NotificationEventTypeCode == NotificationEventType.INDIV));
            if(!willSendtoSubmitter)
            {
                notificationEventType = notificationEvent.NotificationEventType;
                QmsWorkitemcomment comment = ehriErrorRepository.RetrieveLatestComment(ehriError.Id);
                switch(notificationEventType.NotificationEventTypeCode)
                {
                    case NotificationEventType.INDIV:
                        sendIndividualMessage(ehriError,notificationEvent, submitter,comment);
                        break;
                    case NotificationEventType.ORG:
                        sendOrganizationalMessage(ehriError,notificationEvent,comment);
                        break;
                    default:
                        break;
                }
            }            
        }

        internal string formatMessage(DataError ehriError, QmsWorkitemcomment comment)
        {
            template = "EHRI Error ID: {0}<br/>Employee: {1}-{2}<br/>Error Details: {3}<br/>{4}<br/<br/>Updated on: {5}<br/><br/>";
            string dateToUse = ehriError.UpdatedAt.HasValue ? ehriError.UpdatedAt.Value.ToShortDateString() : ehriError.CreatedAt.ToShortDateString();

            message = string.Format(template,ehriError.Id,ehriError.EmplId, ehriError.Employee.FullName,ehriError.QmsErrorCode,ehriError.QmsErrorMessageText,dateToUse);
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

            return message;
        }


        internal void sendIndividualMessage(DataError ehriError, NtfNotificationevent ne, User submitter, QmsWorkitemcomment comment)
        {
            if(submitter.UserId != ehriError.CreatedByUserId.Value) // if the person doing the action is the originator they don't get a message since they did the action
            {

                SecUser user;
                NtfNotification notification = new NtfNotification();
                notification.CreatedAt = DateTime.Now;
                notification.HasBeenRead = 0;
                notification.Title = string.Format(ne.TitleTemplate,ehriError.Id); //HRQMS - EHRI Error Closed ({0})
                notification.WorkitemId = ehriError.Id;
                notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;
                notification.SendAsEmail = 0; //Changed from 1 so it doesn't send
                notification.NotificationEventId = ne.NotificationEventId;
                notification.Message = formatMessage(ehriError,comment);
                switch(ne.NotificationEventCode)
                {
                  
                    case EhriErrorNotificationType.EHRI_Assigned:
                        notification.UserId = ehriError.AssignedToUserId.Value;
                        break;
                    case EhriErrorNotificationType.EHRI_Returned:
                        notification.UserId = ehriError.CreatedByUserId.Value;
                        break;
                    case EhriErrorNotificationType.EHRI_Closed:
                        notification.UserId = ehriError.CreatedByUserId.Value;
                        break;                    
                    default:
                        //not a indivual message type
                        break;
                }

                if(notification.UserId > 0) //0 = System user - no need to notify.
                {
                    context.Add(notification);
                    context.SaveChanges();
                    user = userRepository.RetrieveByUserId(notification.UserId);
                    send(user.EmailAddress,notification.Title,notification.Message);
                }

            }
        }

        internal void sendOrganizationalMessage(DataError ehriError, NtfNotificationevent ne, QmsWorkitemcomment comment)
        {
            List<SecUser> users = getReviewersInOrg(ehriError.AssignedToOrgId.Value);
            string[] emails = new string[users.Count];
            NtfNotification notification = new NtfNotification();
            notification.CreatedAt = DateTime.Now;
            notification.Title = string.Format(ne.TitleTemplate,ehriError.Id);
            notification.Message = formatMessage(ehriError,comment);
            notification.HasBeenRead = 0;
            notification.WorkitemId = ehriError.Id;
            notification.WorkItemTypeCode = WorkItemTypeEnum.CorrectiveActionRequest;
            notification.SendAsEmail = 0; //Changed from 1 so it doesn't send
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
            context.SaveChanges();            
            send(emails,notification.Title,notification.Message);
        }




    }//end class
}//end namespace