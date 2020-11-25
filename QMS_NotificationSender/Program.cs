using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Linq;

using QmsCore.Services;
using QmsCore.Model;
using QmsCore.UIModel;

using MySql.Data.MySqlClient;

namespace QMS_NotificationSender
{
    class Program
    {

        static string logDate = DateTime.Now.ToString("yyyMMdd");

        static int emailsSent = 0;

        static bool shouldSendEventBasedEmails = false;

        static bool shouldSendTimeBasedEmails = false;

        static QMSContext context;
        static ReferenceService referenceService;
        static CorrectiveActionService correctiveActionService;

        static NotificationService notificationService;

        static UserService userService;

        static List<User> Users;

        static StringBuilder stringBuilder = new StringBuilder();

        static List<Notification> notifications = new List<Notification>();

        static string fileDirectory = AppContext.BaseDirectory  + "data\\";


        static List<KeyValuePair<string,string>> settingsList;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(fileDirectory);
                string env = getEnvironment(args);   
                logit("Environmemnt is set to " + env); 
                string configFile = string.Format("qms_appsettings.{0}.json",env);
                logit(string.Format("Using {0} for configuration",configFile));

                Config.Settings.Rebuild(configFile);
                logit("Building Context");
                context = new QMSContext();
                logit("Context Built");

                User user = new UserService().RetrieveByEmailAddress("alfred.ortega@gsa.gov");
                Console.WriteLine(string.Format("User: {0} loaded",user.DisplayName));
                var items = new CorrectiveActionService(context).RetrieveAllForOrganization(user);
                foreach (var item in items)
                {
                    Console.WriteLine(item.DaysOld);
                }




                //logit("Retrieving Settings");
                //settingsList = retrieveSettings(env);
                //logit("Settings defined");    
                //setSettings();                    

                //sendQmsDataEmails();


                //logit("Instantiating Reference Service");
                //referenceService = new ReferenceService(context);
                //logit("Instantiating Corrective Action Service");
                //correctiveActionService = new CorrectiveActionService(context);
                //logit("Instantiating User Service");
                //userService = new UserService(context);
                //logit("Instantiating Notification Service");
                //notificationService = new NotificationService(context);                
                //logit("Loading Active Users");
                //Users = userService.RetrieveActiveUsers();

                //logit("Should Event Based Emails be sent: " + shouldSendEventBasedEmails.ToString());
                //if(shouldSendEventBasedEmails)
                //{
                //    logit("executeEventBasedEmails");
                //    executeEventBasedEmails();
                //}
                //logit("Check to see if time based emails have been sent for today");
                //EmailLog log = referenceService.RetrieveEmailLogByDate(logDate);
                //if(log.EmailLogId == 0) //emails haven't been sent yet today
                //{
                //    logit("Time based emails have not been sent for today");
                //    sendReviewerNotifications();
                //    sendSpecialistNotifications();
                //    logit("saveNotifications");
                //    saveNotifications();
                //    log.SentDate = logDate;
                //    log.SentAmount = emailsSent;
                //    referenceService.SaveEmailLog(log);
                //}
                //else
                //{
                //    logit("Time based emails have sent for today.");
                //}
                //logit("Write Log");
                //System.IO.File.WriteAllText(Config.Settings.LogDirectory + "EmailLog-" + env + ".txt",stringBuilder.ToString());

               
            }
            catch (System.Exception x)
            {
                logit(x.ToString());
                throw x;
            }
        }

        private static void logit(string message)
        {
            string ts = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            string log = string.Format("{0} {1}",ts,message);
            stringBuilder.AppendLine(log);
            Console.WriteLine(log);
        }
#region "Support methods"

        static string getEnvironment(string[] args)
        {
            string retval = "dev";
            try
            {
                string testVal = args[0].Replace("-",string.Empty).ToLower();
                if(testVal == "dev" || testVal == "test" || testVal == "prod")
                {
                    retval = testVal;
                }
                else
                {
                    throw new Exception("Cannot determine the appropriate environment. Defaulting to DEV");
                }   
            }
            catch (System.Exception x)
            {
                logit(x.ToString());
            }
            return retval;
        }

        static string retrieveSettingValue(string setting)
        {

            var value = (from s in settingsList
                        where s.Key == setting
                        select s.Value).SingleOrDefault();

            return value;
        }

        static string retrieveUserEmail(int userid)
        {
            var user = (from u in Users 
                        where u.UserId == userid && u.DeletedAt == null
                        select new {Email = u.EmailAddress}).SingleOrDefault();

            return user.Email;
        }

        static List<string> retrieveOrganizationsReviewersEmails(int orgId)
        {
            List<string> emails = new List<string>();
            var users = userService.RetrieveAllReviewersByOrganziationId(orgId);
            foreach(var user in users)
            {
                emails.Add(user.EmailAddress);
            }
            return emails;
        }

        static List<int> retrieveOrganizationsReviewerIds(int orgId)
        {
            List<int> reviewerIds = new List<int>();
            var users = userService.RetrieveAllReviewersByOrganziationId(orgId);

            foreach(var user in users)
            {
                reviewerIds.Add(user.UserId);
            }            
            return reviewerIds;
        }

        static void setSettings()
        {
            string message = "Setting:{0}\t\tValue:{1}";
            SettingType.SendEventBasedEmail =  retrieveSettingValue(SettingType.SendEventBasedEmail);
            logit(string.Format(message,"Send Event Based Email",SettingType.SendEventBasedEmail));
            shouldSendEventBasedEmails = bool.Parse(SettingType.SendEventBasedEmail); 

            SettingType.EmailServer =  retrieveSettingValue(SettingType.EmailServer);
            SettingType.EmailFooter =  retrieveSettingValue(SettingType.EmailFooter);
            SettingType.CA_INDRAFT_SPECIALIST =  retrieveSettingValue(SettingType.CA_INDRAFT_SPECIALIST);
            SettingType.CA_INDRAFT_REVIEWER =  retrieveSettingValue(SettingType.CA_INDRAFT_REVIEWER);
            SettingType.CA_RETURNED_SPECIALIST =  retrieveSettingValue(SettingType.CA_RETURNED_SPECIALIST);
            SettingType.CA_RETURNED_REVIEWER =  retrieveSettingValue(SettingType.CA_RETURNED_REVIEWER);
            SettingType.CA_ASSIGNED_SC_SPECIALIST =  retrieveSettingValue(SettingType.CA_ASSIGNED_SC_SPECIALIST);
            SettingType.CA_ASSIGNED_PPRM_SPECIALIST =  retrieveSettingValue(SettingType.CA_ASSIGNED_PPRM_SPECIALIST);
            SettingType.CA_ASSIGNED_BRC_SPECIALIST =  retrieveSettingValue(SettingType.CA_ASSIGNED_BRC_SPECIALIST);
            SettingType.CA_ASSIGNED_SC_REVIEWER =  retrieveSettingValue(SettingType.CA_ASSIGNED_SC_REVIEWER);
            SettingType.CA_ASSIGNED_PPRM_REVIEWER =  retrieveSettingValue(SettingType.CA_ASSIGNED_PPRM_REVIEWER);
            SettingType.CA_ASSIGNED_BRC_REVIEWER =  retrieveSettingValue(SettingType.CA_ASSIGNED_BRC_REVIEWER);
            SettingType.CA_PENDING_ASSIGNMENT =  retrieveSettingValue(SettingType.CA_PENDING_ASSIGNMENT);
            SettingType.CA_PENDING_REVIEW =  retrieveSettingValue(SettingType.CA_PENDING_REVIEW);
            SettingType.CA_URL =  retrieveSettingValue(SettingType.CA_URL);
            SettingType.FromEmail = retrieveSettingValue(SettingType.FromEmail);   
            SettingType.SendTimeBasedEmail = retrieveSettingValue(SettingType.SendTimeBasedEmail); 
            shouldSendTimeBasedEmails = bool.Parse( SettingType.SendTimeBasedEmail);       

            logit(string.Format(message,"Send Event Based Email",SettingType.EmailServer));
            logit(string.Format(message,"Email Footer",SettingType.EmailFooter));
            logit(string.Format(message,"CA In Draft (Specialist)",SettingType.CA_INDRAFT_SPECIALIST));
            logit(string.Format(message,"CA In Draft (Reviewer)",SettingType.CA_INDRAFT_REVIEWER));
            logit(string.Format(message,"CA In Returned (Specialist)",SettingType.CA_RETURNED_SPECIALIST));
            logit(string.Format(message,"CA In Returned (Reviewert)",SettingType.CA_RETURNED_REVIEWER));
            logit(string.Format(message,"CA In Assigned (SC Specialist)",SettingType.CA_ASSIGNED_SC_SPECIALIST));
            logit(string.Format(message,"CA In Assigned (PPRM Specialist)",SettingType.CA_ASSIGNED_PPRM_SPECIALIST));
            logit(string.Format(message,"CA In Assigned (BRC Specialist)",SettingType.CA_ASSIGNED_BRC_SPECIALIST));
            logit(string.Format(message,"CA In Assigned (SC Reviewer)",SettingType.CA_ASSIGNED_SC_REVIEWER));
            logit(string.Format(message,"CA In Assigned (PPRM Reviewer)",SettingType.CA_ASSIGNED_PPRM_REVIEWER));
            logit(string.Format(message,"CA In Assigned (BRC Reviewer)",SettingType.CA_ASSIGNED_BRC_REVIEWER));
            logit(string.Format(message,"CA Pending Assignment (Reviewer)",SettingType.CA_PENDING_ASSIGNMENT));
            logit(string.Format(message,"CA Pending Review (Reviewer)",SettingType.CA_PENDING_REVIEW));
            logit(string.Format(message,"CA From URL",SettingType.CA_URL));
            logit(string.Format(message,"From Email",SettingType.FromEmail));   
            logit(string.Format(message,"Send Time Based Email",shouldSendTimeBasedEmails.ToString())); 

        }

        static List<KeyValuePair<string,string>> retrieveSettings(string environment)
        {
            List<KeyValuePair<string,string>> retval = new List<KeyValuePair<string,string>> ();
            var results = (from s in context.SysSetting
                          join t in context.SysSettingtype on s.SettingTypeId equals t.SettingTypeId
                          where t.Deletedat == null
                             && s.Environment == environment
                          select new {
                              Setting = t.SettingCode,  
                              Value = s.SettingValue
                          }).ToList();
            foreach(var result in results)
            {
                retval.Add(new KeyValuePair<string, string>(result.Setting,result.Value));
            }

            return retval;
        }

        static int retrieveStatusId(string status)
        {
            return referenceService.RetrieveStatusByStatusCode(status).StatusId;

        }
        static void executeEventBasedEmails()
        {

            var items = notificationService.RetrieveNotificationForDistribution();
            List<MailMessage> messages = new List<MailMessage>();
            List<int> notificationIds = new List<int>(); 
            string redirectUrl = retrieveSettingValue(SettingType.CA_URL);
            foreach(var item in items)
            {
                notificationIds.Add(item.NotificationId);
                logit("Send email titled " + item.Title + " to " + item.NotificationRecipient.EmailAddress);
                MailMessage message = new MailMessage();
                message.To.Add(item.NotificationRecipient.EmailAddress);
                message.Subject = item.Title;
                message.From = new MailAddress(SettingType.FromEmail);
                message.Body = item.Message + string.Format(SettingType.CA_URL,item.WorkitemId);
                message.IsBodyHtml = true;
                messages.Add(message);

                item.SentAt = DateTime.Now;

            }

            sendEmails(messages);
            MarkAsSent(notificationIds);
            logit(items.Count + " notification sent");
         
        }



        static void sendOrganzationEmail(List<string> emailAddresses, List<int> reviewerIds, int correctiveActionId, int daysOld, string subjectLine)
        {
            List<MailMessage> messages = new List<MailMessage>();

            foreach(var reviewerId in reviewerIds)
            {
                Notification notification = new Notification();
                notification.HasBeenRead = false;
                notification.Message =  string.Format("Corrective Action {0} is {1} days old",correctiveActionId,daysOld);
                notification.NotificationEventId = 9;
                notification.SendAsEmail = false;
                notification.HasBeenRead = false;
                notification.SentAt = DateTime.Now;
                notification.Title = string.Format(subjectLine,correctiveActionId);
                notification.UserId = reviewerId;
                notification.WorkitemId = correctiveActionId;
                notification.WorkItemType = WorkItemTypeEnum.CorrectiveActionRequest;
                notifications.Add(notification);
            }

            if(shouldSendTimeBasedEmails)
            {
                foreach(var emailAddress in emailAddresses)
                {
                    MailMessage message = new MailMessage();
                    message.To.Add(emailAddress);
                    message.Subject = string.Format(subjectLine,correctiveActionId);
                    message.From = new MailAddress(SettingType.FromEmail);
                    message.Body = string.Format("Corrective Action {0} is {1} {2}",correctiveActionId,daysOld, SettingType.EmailFooter);
                    message.IsBodyHtml = true;
                    messages.Add(message);
                }
                sendEmails(messages);
            }    
        }

        static void sendIndividualEmail(string emailAddress, int userId, int correctiveActionId, int daysOld, string subjectLine)
        {
            if(shouldSendTimeBasedEmails)
            {
                MailMessage message = new MailMessage();
                message.To.Add(emailAddress);
                message.Subject = string.Format(subjectLine,correctiveActionId);
                message.From = new MailAddress(SettingType.FromEmail);
                message.Body = string.Format("Corrective Action {0} is {1} {2}",correctiveActionId,daysOld, SettingType.EmailFooter);
                message.IsBodyHtml = true;
                sendEmail(message);
            }

            Notification notification = new Notification();
            notification.HasBeenRead = false;
            notification.Message =  string.Format("Corrective Action {0} is {1} days old",correctiveActionId,daysOld);
            notification.NotificationEventId = 9;
            notification.SendAsEmail = false;
            notification.SentAt = DateTime.Now;
            notification.Title = string.Format(subjectLine,correctiveActionId);
            notification.UserId = userId;
            notification.WorkitemId = correctiveActionId;
            notification.WorkItemType = WorkItemTypeEnum.CorrectiveActionRequest;
            notifications.Add(notification);
        }

        static List<CorrectiveAction> retrieveCorrectiveActionByStatusAndAge(int statusId, int daysOld)
        {
            logit(string.Format("Retrieving CorrectiveActions in Status: {0} that are {1} days old",statusId,daysOld));            
            return correctiveActionService.RetrieveAllByStatusAndAge(statusId, daysOld).ToList();
        }

        static List<CorrectiveAction> retrieveCorrectiveActionByStatusAndAge(int statusId, int daysOld, int orgId)
        {
            logit(string.Format("Retrieving CorrectiveActions in Status: {0} that are {1} days old for org {2}",statusId,daysOld,orgId));            
            return correctiveActionService.RetrieveAllByStatusAndAge(statusId, daysOld,orgId).ToList();
        }

#endregion

        static void sendReviewerNotifications()
        {
            logit("executeSendNotificationsInPendingReview");
            executeSendNotificationsInPendingReview();
            logit("executeSendNotificationsInPendingAssignment");
            executeSendNotificationsInPendingAssignment();
            logit("executeSendNotificationsInRerouted");
            executeSendNotificationsInRerouted();
            logit("executeSendNotificationsInDraftReviewer");
            executeSendNotificationsInDraftReviewer();
            logit("executeSendNotificationsInReturnedReviewer");
            executeSendNotificationsInReturnedReviewer();
            logit("executeSendNotificationsInAssignedSCReviewer");
            executeSendNotificationsInAssignedSCReviewer();
            logit("executeSendNotificationsInAssignedBRCReviewer");
            executeSendNotificationsInAssignedBRCReviewer();
            logit("executeSendNotificationsInAssignedPPRMReviewer");
            executeSendNotificationsInAssignedPPRMReviewer();

        }

        static void sendSpecialistNotifications()
        {
            logit("executeSendNotificationsInDraftSpecialist");
            executeSendNotificationsInDraftSpecialist();
            logit("executeSendNotificationsInReturnedSpecialist");
            executeSendNotificationsInReturnedSpecialist();
            logit("executeSendNotificationsInAssignedSCSpecialist");
            executeSendNotificationsInAssignedSCSpecialist();
            logit("executeSendNotificationsInAssignedBRCSpecialist");
            executeSendNotificationsInAssignedBRCSpecialist();
            logit("executeSendNotificationsInAssignedPPRMSpecialist");
            executeSendNotificationsInAssignedPPRMSpecialist();
            
            
        }



#region "Pending Review"

        static void executeSendNotificationsInPendingReview()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_PENDING_REVIEW);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Pending Review: " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Review");                    
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }            
        }        

#endregion

#region "Pending Assignment or Rerouted"

        static void executeSendNotificationsInPendingAssignment()
        {
            int statusId = retrieveStatusId(StatusType.UNASSIGNED);
            int daysOld = int.Parse(SettingType.CA_PENDING_ASSIGNMENT);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Pending Assignment: " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Assignment");                  
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }   

       
        }        

        //Rerouted
        static void executeSendNotificationsInRerouted()
        {
            int statusId = retrieveStatusId(StatusType.REROUTED);
            int daysOld = int.Parse(SettingType.CA_PENDING_REVIEW);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Rerouted: " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               

        }                

#endregion

#region "In Draft"

        static void executeSendNotificationsInDraftSpecialist()
        {
            int statusId = retrieveStatusId(StatusType.DRAFT);
            int daysOld = int.Parse(SettingType.CA_INDRAFT_SPECIALIST);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found In Draft (Specialist): " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var email = retrieveUserEmail(correctiveAction.CreatedByUserId.Value);  //Created by 
                    sendIndividualEmail(email,correctiveAction.CreatedByUserId.Value,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Still in Draft");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.ToString());
                }
            }               
        }

        static void executeSendNotificationsInDraftReviewer()
        {
            int statusId = retrieveStatusId(StatusType.DRAFT);
            int daysOld = int.Parse(SettingType.CA_INDRAFT_SPECIALIST);

            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found In Draft or Review (Specialist): " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.CreatedAtOrgId);
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.CreatedAtOrgId);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Still in Draft");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }        

#endregion

#region "Returned"
        static void executeSendNotificationsInReturnedSpecialist()
        {
            int statusId = retrieveStatusId(StatusType.RETURNED);
            int daysOld = int.Parse(SettingType.CA_RETURNED_SPECIALIST);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Returned (Specialist): " + correctiveActions.Count);
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var email = retrieveUserEmail(correctiveAction.CreatedByUserId.Value);  //Created by 
                    sendIndividualEmail(email,correctiveAction.CreatedByUserId.Value,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }

        static void executeSendNotificationsInReturnedReviewer()
        {
            int statusId = retrieveStatusId(StatusType.RETURNED);
            int daysOld = int.Parse(SettingType.CA_RETURNED_REVIEWER);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Returned (Reviewer): " + correctiveActions.Count);

            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.CreatedAtOrgId);  //Created by 
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }

#endregion

#region "Assigned"

        // **************************************************************
        // **************************************************************
        // Service Centers
        // **************************************************************
        // **************************************************************
        static void executeSendNotificationsInAssignedSCSpecialist()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_SC_SPECIALIST);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Assigned (SC Specialist): " + correctiveActions.Count);

            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var email = retrieveUserEmail(correctiveAction.AssignedToUserId.Value);
                    sendIndividualEmail(email,correctiveAction.AssignedToUserId.Value,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }

        static void executeSendNotificationsInAssignedSCReviewer()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_SC_REVIEWER);
            List<Organization> organizations = referenceService.RetrieveServiceCenters();
            foreach(Organization organization in organizations)
            {
                var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld,organization.OrgId);
                logit("Corrective Actions Found Assigned (SC Reviewer): " + correctiveActions.Count.ToString() + " in " + organization.OrgLabel);
                foreach(var correctiveAction in correctiveActions)
                {
                    try
                    {
                        var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);
                        var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                        sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");                
                    }
                    catch (System.Exception x)
                    {
                        logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                    }
                }               
            }


        }
        // **************************************************************
        // **************************************************************
        // BRC
        // **************************************************************
        // **************************************************************
        static void executeSendNotificationsInAssignedBRCSpecialist()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_BRC_SPECIALIST);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Assigned (BRC Specialist): " + correctiveActions.Count);

            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var email = retrieveUserEmail(correctiveAction.AssignedToUserId.Value);
                    sendIndividualEmail(email,correctiveAction.AssignedToUserId.Value,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");               
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }


        }

        static void executeSendNotificationsInAssignedBRCReviewer()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_BRC_REVIEWER);
            int orgId = referenceService.RetrieveOrgByOrgCode("BRC").OrgId;
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld,orgId);
            logit("Corrective Actions Found Assigned (BRC Reviewer): " + correctiveActions.Count);

            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);                    
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");                
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }

        // **************************************************************
        // **************************************************************
        // PPRM
        // **************************************************************
        // **************************************************************
        static void executeSendNotificationsInAssignedPPRMSpecialist()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_PPRM_SPECIALIST);
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld);
            logit("Corrective Actions Found Assigned (PPRM Specialist): " + correctiveActions.Count);

            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                var email = retrieveUserEmail(correctiveAction.AssignedToUserId.Value);
                sendIndividualEmail(email,correctiveAction.AssignedToUserId.Value,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");               
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }               
        }

        static void executeSendNotificationsInAssignedPPRMReviewer()
        {
            int statusId = retrieveStatusId(StatusType.ASSIGNED);
            int daysOld = int.Parse(SettingType.CA_ASSIGNED_PPRM_REVIEWER);
            int orgId = referenceService.RetrieveOrgByOrgCode("PPRM").OrgId;
            var correctiveActions = retrieveCorrectiveActionByStatusAndAge(statusId,daysOld,orgId);
            logit("Corrective Actions Found Assigned (PPRM Reviewer): " + correctiveActions.Count);
        
            foreach(var correctiveAction in correctiveActions)
            {
                try
                {
                    var reviewerIds = retrieveOrganizationsReviewerIds(correctiveAction.AssignedToOrgId.Value);                    
                    var emails = retrieveOrganizationsReviewersEmails(correctiveAction.AssignedToOrgId.Value);
                    sendOrganzationEmail(emails,reviewerIds,correctiveAction.Id,daysOld,"HRQMS Corrective Action {0} Awaiting Action");              
                }
                catch (System.Exception x)
                {
                    logit("Error with CA#  " + correctiveAction.Id + " " + x.Message);
                }
            }   
        }        

#endregion






        static void sendEmails(List<MailMessage> messages)
        {
            foreach(MailMessage message in messages)
            {
               sendEmail(message);
            }
        }



        static void sendEmail(MailMessage message)
        {
            emailsSent++;
            string header = DateTime.Now.ToShortTimeString() + " Email Info:";
            logit(header);
            stringBuilder.AppendLine(header);
            string subjectLine = string.Format("Subject Line: {0}<br/>",message.Subject);
            logit(subjectLine);
            stringBuilder.AppendLine(subjectLine);
            string recipient = string.Format("Recipeient: {0}<br/>",message.To[0].Address);
            logit(recipient);
            stringBuilder.AppendLine(recipient);
            stringBuilder.AppendLine(string.Format("Body:{0}<br/>",message.Body));
            stringBuilder.AppendLine("<br/>");
//            notificationService.SendEmail(message);
        }        

        static void saveNotifications()
        {
            foreach(Notification notification in notifications)
            {
                try
                {
                    notificationService.Insert(notification);
                }
                catch (System.Exception x)
                {
                    logit(string.Format("Failed to save notification ({0})\t{1}",notification.Title,x.Message));
                }
            }
        }

        static void MarkAsSent(List<int> notificationIds)
        {
            foreach(int notificationId in notificationIds)
            {
                MarkAsSent(notificationId);
            }
        }

        static void MarkAsSent(int notificationId)
        {
            notificationService.MarkAsSent(notificationId);
        }

#region "Send Data Files"


        static void sendQmsDataEmails()
        {
            if(!System.IO.Directory.Exists(fileDirectory))
                System.IO.Directory.CreateDirectory(fileDirectory);
    
            string dataErrorFile = fileDirectory + "dataErrors.csv";
            string dataErrorView = "qms_DataMetricsForErrorType";
            string dataErrorHeader = "ErrorType, Count, Org";

            createDataFiles(dataErrorFile, dataErrorView,dataErrorHeader);

            string dataRecordFile = fileDirectory + "dataRecordFile.csv"; 
            string dataRecordView = "qms_DataMetricsRecordsList";
            string dataRecordHeader = "ID,EMPLID,Employee Name,POI,Office Symbol,NOA,Created At,Created,Closed,Current Status,Days Open,Last Date to PPRM,Days at PPRM,Returned Count,Submitter,Resolver,Assigned To,Pay Impacting";

            createDataFiles(dataRecordFile,dataRecordView,dataRecordHeader);

            MailMessage mailMessage = new MailMessage();
            mailMessage.Attachments.Add(new Attachment(dataErrorFile));
            mailMessage.Attachments.Add(new Attachment(dataRecordFile));
            mailMessage.To.Add("alfred.ortega@gsa.gov");
            mailMessage.Subject = "Daily QMS Metrics " + logDate;
            mailMessage.From = new MailAddress(SettingType.FromEmail);

            SmtpClient client = new SmtpClient(SettingType.EmailServer);
            client.Send(mailMessage);

        }

        static void createDataFiles(string fileName, string viewName, string header)
        {
            StringBuilder records = new StringBuilder();
            records.AppendLine(header);
            using(MySqlConnection connection = new MySqlConnection(Config.Settings.ReconDB))
            {
                string sql = "select * from aca." + viewName;
                connection.Open();
                using(MySqlCommand command = new MySqlCommand(sql,connection))
                {
                    MySqlDataReader dataReader = command.ExecuteReader();
                    int fieldCount = dataReader.FieldCount;
                    while(dataReader.Read())
                    {
                        string record = string.Empty;
                        for(int i = 0; i < fieldCount; i++)
                        {
                            record += dataReader[i].ToString() + ",";
                        }
                        record = record.Substring(0,record.Length-1); // remove trailing comma
                        records.AppendLine(record);
                    }
                }//end command
                connection.Close();
            }//end connection
            writeCSVFile(fileName,records.ToString());
            
        }

        static void writeCSVFile(string fileName, string body)
        {
            if(System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            System.IO.File.WriteAllText(fileName,body); 

        }



#endregion


    }//end class
}//end namespace
