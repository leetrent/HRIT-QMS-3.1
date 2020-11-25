using System;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;
using QmsCore.Repository;
using QmsCore.QmsException;
using System.Linq;

namespace QmsCore.Services
{
    public class UserService : IUserService
    {
        UserRepository repository;
        SecurityLogService securityLogService;
        RoleService roleService;

        public UserService()
        {
            repository = new UserRepository();
            securityLogService = new SecurityLogService();
            roleService = new RoleService();
            
        }

        public UserService(IUserRepository userRepository)
        {
            repository = (UserRepository)userRepository;
            securityLogService = new SecurityLogService();        }

        public UserService(QMSContext qmsContext)
        {
            repository = new UserRepository(qmsContext);
            securityLogService = new SecurityLogService(qmsContext);
            roleService = new RoleService(qmsContext);
        }

        public User RetrieveByEmailAddress(string emailAddress)
        {
            var secuser = repository.RetrieveByEmailAddress(emailAddress);
            if(secuser!=null)
            {
                User user = new User(secuser);
                securityLogService.UserNavigation(user);
                return user;
            }
            else
            {
                securityLogService.RecordFailedLogin(emailAddress);
                throw new UserNotFoundException(string.Format("User with email '{0}' not found",emailAddress));
            }
        }

        public User RetrieveByEmailAddress(string emailAddress, string uri)
        {
            var secuser = repository.RetrieveByEmailAddress(emailAddress);
            if(secuser!=null)
            {
                User user = new User(secuser);
                securityLogService.UserNavigationWithUri(user,uri);
                return user;
            }
            else
            {
                securityLogService.RecordFailedLogin(emailAddress);
                throw new UserNotFoundException(string.Format("User with email '{0}' not found",emailAddress));
            }
        }


        public User RetrieveByEmailAddress(string emailAddress, bool IsInitialLogIn = false)
        {
            var secuser = repository.RetrieveByEmailAddress(emailAddress);
            if(secuser!=null)
            {
                User user = new User(secuser);
                securityLogService.LoginUser(user);
                return user;
            }
            else
            {
                securityLogService.RecordFailedLogin(emailAddress);                
                throw new UserNotFoundException(string.Format("User with email '{0}' not found",emailAddress));
            }
        }

        public User RetrieveByUserId(int userId)
        {
            SecUser secUser = repository.RetrieveByUserId(userId);
            return new User(secUser, true, true);
        }


        public List<User> RetrieveUsersByOrganizationId(int organizationId)
        {
            var secusers = repository.RetrieveUsersByOrganziationId(organizationId).ToList();
            List<User> users = new List<User>();
            foreach(var secuser in secusers)
            {
                users.Add(new User(secuser));
            }
            return users;
        }

        public List<User> RetrieveActiveUsers()
        {
            var secusers = repository.RetrieveAllActiveUsers();
            List<User> users = new List<User>();
            foreach(var secuser in secusers)
            {
                users.Add(new User(secuser,true,true));
            }
            return users;
        }

        public List<User> RetrieveInactiveUsers()
        {
            var secusers = repository.RetrieveAllInactiveUsers();
            List<User> users = new List<User>();
            foreach(var secuser in secusers)
            {
                users.Add(new User(secuser,true,true));
            }
            return users;
        }




        public List<User> RetrieveAllReviewersByOrganziationId(int organizationId)
        {
            var secusers = repository.RetrieveAllReviewersByOrganizationId(organizationId);
            List<User> users = new List<User>();
            foreach(var secuser in secusers)
            {
                users.Add(new User(secuser,false,false));
            }
            return users;
        }


        List<User> RetrieveUsersForOrganization(int orgId)
        {
            var secusers = repository.RetrieveUsersByOrganziationId(orgId);
            List<User> users = new List<User>();
            foreach(var secuser in secusers)
            {
                users.Add(new User(secuser,false,false));
            }
            return users;
        }


        public int CreateUser(User newUser, User submitter)
        {
            securityLogService.EntityUpdatedOrCreated(SecurityLogTypeEnum.USER_CREATED,newUser,submitter);
            newUser.CreatedAt = DateTime.Now;
            hyrdateRoles(newUser);
            return repository.CreateUser(newUser.SecUser());
        }

        public void UpdateUser(User updatedUser, User submitter)
        {
            if(updatedUser.DeletedAt.HasValue)
            {
               securityLogService.EntityUpdatedOrCreated(SecurityLogTypeEnum.USER_DEACTIVATED,updatedUser,submitter);
            }
            else
            {
                securityLogService.EntityUpdatedOrCreated(SecurityLogTypeEnum.USER_UPDATED,updatedUser,submitter);
            }
            hyrdateRoles(updatedUser);
            auditRoles(updatedUser,submitter);
            repository.Update(updatedUser.SecUser());
        }

        public void ReactivateUser(int UserId, User submitter)
        {
            SecUser user = repository.RetrieveByUserId(UserId);
            User reactivatedUser = RetrieveByUserId(UserId);
            user.DeletedAt = null;
            securityLogService.EntityUpdatedOrCreated(SecurityLogTypeEnum.USER_REACTIVATED, reactivatedUser, submitter);
            repository.context.SaveChanges();
        }

        public void DeactivateUser(int UserId, User submitter)
        {
            SecUser user = repository.RetrieveByUserId(UserId);
            User deactivatedUser = RetrieveByUserId(UserId);
            user.DeletedAt = DateTime.Now;
            securityLogService.EntityUpdatedOrCreated(SecurityLogTypeEnum.USER_DEACTIVATED,deactivatedUser,submitter);
            repository.context.SaveChanges();
        }

        private void hyrdateRoles(User updatedUser)
        {
            foreach(var userRole in updatedUser.UserRoles)
            {
                userRole.Role = roleService.RetrieveRole(userRole.RoleId);
            }
        }

        private void auditRoles(User updatedUser, User submitter)
        {
            var updatedRoles = updatedUser.UserRoles;
            var existingRoles = RetrieveByUserId(updatedUser.UserId).UserRoles;
            foreach(var updatedRole in updatedRoles)//check for new roles
            {
                if(!existingRoles.Contains(updatedRole))
                    securityLogService.AssignRoleToUser(updatedRole.Role,updatedUser,submitter);
            }

            foreach(var existingRole in existingRoles)//check for removed roles
            {
                if(!updatedRoles.Contains(existingRole))
                    securityLogService.RemoveRoleFromUser(existingRole.Role,updatedUser,submitter);
            }
        }


    }
}