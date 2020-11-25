using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface ISecurityLogService
    {
        void EntityUpdatedOrCreated(string SecurityLogType, ILoggable entity, User submitter);
        void AddPermissionToRole(Role role, Permission permission, User submitter);
        void RemovePermissionFromRole(Role role, Permission permission, User submitter);
        void AssignRoleToUser(Role role, User assignee, User submitter);
        void RemoveRoleFromUser(Role role, User assignee, User submitter);
        void LoginUser(User user);
        void UserNavigation(User user);
        void RecordFailedLogin(string emailAddress);
        void UserNavigationWithUri(User user, string uri);
    }//end interface
}//end namespace