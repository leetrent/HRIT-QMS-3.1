using System;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;
using QmsCore.Repository;
using QmsCore.QmsException;
using System.Linq;

namespace QmsCore.Services
{
    public class SecurityLogService : ISecurityLogService
    {
        SecurityLogRepository repository;

        SecSecuritylogtype logType;
        SecSecuritylog log;

        string logMessage = string.Empty;

        public SecurityLogService()
        {
            repository = new SecurityLogRepository();
        }

        public SecurityLogService(QMSContext context)
        {
            repository = new SecurityLogRepository(context);
        }

        internal void Save(SecurityLog securityLog)
        {

        }

        
        private void setSecurityLogType(string securityLogTypeCode)
        {
            logType = repository.RetrieveSecurityLogType(securityLogTypeCode);
        }
        public void EntityUpdatedOrCreated(string SecurityLogType, ILoggable entity, User submitter)
        {
            setSecurityLogType(SecurityLogType);
            string oldLabel = string.Empty;
            switch(SecurityLogType)
            {
                case "USER_UPDATED": //                SecurityLogTypeEnum.USER_UPDATED:
                case "USER_CREATED":
                case "USER_DEACTIVATED":
                case "USER_REACTIVATED":
                case "ROLE_CREATED":
                case "ROLE_DEACTIVATED":
                case "ROLE_REACTIVATED":
                case "ORG_CREATED":
                case "ORG_DEACTIVATED":
                case "ORG_REACTIVATED":
                case "PERM_CREATED":
                case "PERM_DEACTIVATED":
                case "PERM_REACTIVATED":
                    logMessage = string.Format(logType.SecurityLogTemplate,entity.Label);
                    break;
                case "ROLE_UPDATED":
                    oldLabel = getRoleLabel(entity.ID);
                    logMessage = string.Format(logType.SecurityLogTemplate,oldLabel,entity.Label);
                    break;
                case "ORG_UPDATED":
                    oldLabel = getOrgLabel(entity.ID);
                    logMessage = string.Format(logType.SecurityLogTemplate,oldLabel,entity.Label);
                    break;
                case "PERM_UPDATED":
                    oldLabel = getPermissionLabel(entity.ID);
                    logMessage = string.Format(logType.SecurityLogTemplate,oldLabel,entity.Label);
                    break;                    
                default:
                    throw new Exception();
            }
            saveMessage(submitter,logMessage,SecurityLogType,entity);
        }

        private void saveMessage(User submitter, string logMessage,string SecurityLogType, ILoggable entity)
        {
            log = new SecSecuritylog();
            log.ActionTakenByUserId = submitter.UserId;
            log.SecurityLogTypeId = logType.SecurityLogTypeId;
            log.Description = logMessage;
            log.ActiontakenOnItemId = entity.ID;
            log.CreatedAt = DateTime.Now;
            repository.SaveEntry(log);            

        }

        public void AddPermissionToRole(Role role, Permission permission, User submitter)
        {
            setSecurityLogType(SecurityLogTypeEnum.PERM_ADDED_TO_ROLE);
            logMessage = string.Format(logType.SecurityLogTemplate,permission.PermissionCode,role.RoleCode);
            saveMessage(submitter,logMessage,SecurityLogTypeEnum.PERM_ADDED_TO_ROLE,role);
        }
        public void RemovePermissionFromRole(Role role, Permission permission, User submitter)
        {
            setSecurityLogType(SecurityLogTypeEnum.PERM_REMOVED_FROM_ROLE);
            logMessage = string.Format(logType.SecurityLogTemplate,permission.PermissionCode,role.RoleCode);
            saveMessage(submitter,logMessage,SecurityLogTypeEnum.PERM_REMOVED_FROM_ROLE,permission);
        }

        public void AssignRoleToUser(Role role, User assignee, User submitter)
        {
            setSecurityLogType(SecurityLogTypeEnum.USER_ASSIGNED_ROLE);
            logMessage = string.Format(logType.SecurityLogTemplate,assignee.DisplayName,role.RoleCode);            
            saveMessage(submitter,logMessage,SecurityLogTypeEnum.USER_ASSIGNED_ROLE,assignee);
        }

        public void RemoveRoleFromUser(Role role, User assignee, User submitter)
        {
            setSecurityLogType(SecurityLogTypeEnum.USER_UNASSIGNED_ROLE);
            logMessage = string.Format(logType.SecurityLogTemplate,assignee.DisplayName,role.RoleCode);  
            saveMessage(submitter,logMessage,SecurityLogTypeEnum.USER_UNASSIGNED_ROLE,assignee);             
        }

        public void LoginUser(User user)
        {
            setSecurityLogType(SecurityLogTypeEnum.USER_LOGIN_SUCCESS);
            logMessage = string.Format(logType.SecurityLogTemplate,user.DisplayName);
            saveMessage(user,logMessage,SecurityLogTypeEnum.USER_UNASSIGNED_ROLE,user);               
        }

        public void UserNavigation(User user)
        {
            setSecurityLogType(SecurityLogTypeEnum.USER_LOGIN_NAVIGATE);
            logMessage = string.Format(logType.SecurityLogTemplate,user.DisplayName);
            saveMessage(user,logMessage,SecurityLogTypeEnum.USER_UNASSIGNED_ROLE,user);   
        }


        public void RecordFailedLogin(string emailAddress)
        {
            setSecurityLogType(SecurityLogTypeEnum.USER_LOGIN_FAIL);
            logMessage = string.Format(logType.SecurityLogTemplate,emailAddress);
            User dummy = new User() {UserId = 0, DisplayName = "INVALID USER"};
            saveMessage(dummy,logMessage,SecurityLogTypeEnum.USER_UNASSIGNED_ROLE,dummy);               
        }

        public void UserNavigationWithUri(User user, string uri)
        {
            //USER_LOGIN_NAVIGATE_TO_PAGE
            setSecurityLogType(SecurityLogTypeEnum.USER_LOGIN_NAVIGATE_TO_PAGE);
            logMessage = string.Format(logType.SecurityLogTemplate,user.DisplayName, uri);
            saveMessage(user,logMessage,SecurityLogTypeEnum.USER_LOGIN_NAVIGATE_TO_PAGE,user);             
        }


#region "retrieval labels"

    private string getPermissionLabel(int id)
    {
        return new PermissionService().RetrievePermission(id).PermissionLabel;
    }

    private string getRoleLabel(int id)
    {
       return new RoleService().RetrieveRole(id).RoleLabel;
    }

    private string getOrgLabel(int id)
    {
        return new OrganizationService().RetrieveOrganization(id).OrgLabel;
    }


#endregion


    }//end class
}//end namespace