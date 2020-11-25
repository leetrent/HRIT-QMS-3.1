using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class UserRepository : IUserRepository
    {
        internal QMSContext context;
        public UserRepository()
        {
            context = new QMSContext();
        }

        internal UserRepository(QMSContext qmsContext)
        {
            context = qmsContext;
        }


        public SecUser RetrieveByEmailAddress(string emailAddress)
        {
            return context.SecUser.Where(u => u.EmailAddress == emailAddress)
                                  .Include(u => u.Manager)
                                  .Include(u => u.Org)
                                  .Include(u => u.SecUserRole).ThenInclude(u => u.Role).ThenInclude(r => r.SecRolePermission).ThenInclude(r => r.Permission).SingleOrDefault();
        }

        public IQueryable<SecUser> RetrieveUsersByOrganziationId(int organizationId)
        {
            return context.SecUser.Where(u => u.DeletedAt == null && u.OrgId == organizationId).Include(u => u.Org).Include(u => u.SecUserRole);
        }

        public IQueryable<SecUser> RetrieveAllActiveUsers()
        {
            return context.SecUser.Where(u => u.DeletedAt == null  && u.UserId > 0)
                                  .Include(u => u.Org)
                                  .Include(u => u.Manager)
                                  .Include(u => u.SecUserRole)
                                  .OrderBy(u => u.EmailAddress).AsNoTracking();
        }

        public IQueryable<SecUser> RetrieveAllInactiveUsers()
        {
            return context.SecUser.Where(u => u.DeletedAt != null  && u.UserId > 0)
                                  .Include(u => u.Org)
                                  .Include(u => u.Manager)
                                  .Include(u => u.SecUserRole)
                                  .OrderBy(u => u.EmailAddress).AsNoTracking();
        }


        public List<SecUser> RetrieveAllReviewersByOrganizationId(int organizationId)
        {
            List<SecUser> retval = new List<SecUser>();
            var users = context.SecUser.Where(u => u.DeletedAt == null && u.OrgId == organizationId).Include(u => u.SecUserRole).ThenInclude(o => o.Role);
            foreach(var user in users)
            {
                foreach(var orgRole in user.SecUserRole)
                {
                    if(orgRole.Role.RoleCode.ToUpper().Contains("REVIEWER"))
                    {
                        retval.Add(user);
                    }
                }
            }
            return retval;            
        }        

        internal SecUser RetrieveByUserId(int userId)
        {
            return context.SecUser.Where(u => u.UserId == userId)
                                  .Include(u => u.Org)
                                  .Include(u => u.Manager)
                                  .Include(u => u.SecUserRole).ThenInclude(u => u.Role).ThenInclude(r => r.SecRolePermission).ThenInclude(r => r.Permission)
                                  .SingleOrDefault(); 
        }


        public int CreateUser(SecUser secUser)
        {
            context.SecUser.Add(secUser);
            foreach(var sur in secUser.SecUserRole)
            {
                context.SecUserRole.Add(sur);
            }
            context.SaveChanges();
            return secUser.UserId;
        }

        internal void Update(SecUser secUser)
        {
            mergeAndClean(secUser);
            clearContext();
            context.SecUser.Update(secUser);
            foreach (var sur in secUser.SecUserRole)
            {
                context.SecUserRole.Add(sur);
            }
            //update(oldUser, secUser);
            context.SaveChanges();
        }

        private void mergeAndClean(SecUser secUser)
        {
            SecUser oldUser = RetrieveByUserId(secUser.UserId);
            secUser.CreatedAt = oldUser.CreatedAt;
            secUser.UpdatedAt = DateTime.Now;
            purgeRoles(oldUser);
        }

        private void clearContext()
        {
            var entries = context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }


        private void purgeRoles(SecUser user)
        {
            var roles = context.SecUserRole.Where(r => r.UserId == user.UserId);
            context.SecUserRole.RemoveRange(roles);
            context.SaveChanges();
        }

        internal void update(SecUser oldUser, SecUser newUser)
        {
            newUser.CreatedAt = oldUser.CreatedAt;
            context.Entry(oldUser).State = EntityState.Deleted;
            context.Entry(newUser).State = EntityState.Modified;
            newUser.UpdatedAt = DateTime.Now;
            context.SecUser.Update(newUser);
        }


    }
}