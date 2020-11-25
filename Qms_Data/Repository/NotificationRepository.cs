using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public class NotificationRepository
    {
        private QMSContext context = null;

        public NotificationRepository(){
            context = new QMSContext();
        }

        public NotificationRepository(QMSContext qmsContext)
        {
            context = qmsContext;
        }

        public void Delete(NtfNotification notification)
        {
            notification.DeletedAt = DateTime.Now;
            Update(notification);
        }

        public int Update(NtfNotification newNotification)
        {
            int retval = -1;
            try
            {
                NtfNotification oldNotification = RetrieveNotificationById(newNotification.NotificationId);
                context.Entry(oldNotification).State = EntityState.Deleted;
                context.Entry(newNotification).State = EntityState.Modified;
                retval = context.SaveChanges();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
            return retval;


        }

        public int Insert(NtfNotification notification)
        {
            context.Add(notification);
            return context.SaveChanges();
        }


#region "Retrievals"

        public NtfNotification RetrieveNotificationById(int id)
        {
            return context.NtfNotification.AsNoTracking().Where(n => n.NotificationId == id).Include(n => n.WorkItemTypeCodeNavigation).SingleOrDefault();
        }

        public NtfNotificationevent RetrieveNotificationEventByCode(string notificationEventCode)
        {
            return context.NtfNotificationevent.AsNoTracking().Where(n => n.NotificationEventCode == notificationEventCode && n.DeletedAt == null).Include(n=> n.NotificationEventType).SingleOrDefault();
        }

        internal int RetrieveNotificationCountByUserId(int userId, bool onlyRead)
        {
            return RetrieveNotificationByUserId(userId,onlyRead).Count();
        }

        internal IQueryable<NtfNotification> RetrieveNotificationByUserId(int userId, bool onlyUnread)
        {
            if(onlyUnread)
            {
                return context.NtfNotification.AsNoTracking().Where(n => n.UserId == userId && n.ReadAt == null && n.DeletedAt == null).Include(n => n.User).Include(n => n.WorkItemTypeCodeNavigation); 
            }
            else
            {
                return context.NtfNotification.AsNoTracking().Where(n => n.UserId == userId && n.DeletedAt == null).Include(n => n.User).Include(n => n.WorkItemTypeCodeNavigation);
            }
            
        }

        internal IQueryable<NtfNotification> RetrieveNotificationForDistribution()
        {
            return context.NtfNotification.AsNoTracking().Where(n => n.SendAsEmail == 1 && n.SentAt == null && n.DeletedAt == null).Include(n => n.User);
        }




#endregion




    }//end class
}//end namespace