using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IUserService
    {
        User RetrieveByEmailAddress(string emailAddress);
        User RetrieveByEmailAddress(string emailAddress, bool IsInitialLogIn = false);
        List<User> RetrieveUsersByOrganizationId(int organizationId);
        List<User> RetrieveActiveUsers();
        List<User> RetrieveInactiveUsers();
        List<User> RetrieveAllReviewersByOrganziationId(int organizationId);
        User RetrieveByUserId(int userId);
        int CreateUser(User newUser, User submitter);
        void UpdateUser(User updatedUser, User submitter);
        void DeactivateUser(int UserId, User submitter);
        void ReactivateUser(int UserId, User submitter);
    }
}