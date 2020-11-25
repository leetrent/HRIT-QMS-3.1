using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;

namespace QmsCore.Repository
{

    public class ReferenceRepository : IReferenceRepository
    {
        internal QMSContext context;

        internal ReferenceRepository()
        {
            context = new QMSContext();
        }

        internal ReferenceRepository(QMSContext qmsContext)
        {
            context = qmsContext;
        }

        public IQueryable<QmsNatureofaction> RetrieveNatureOfActions()
        {
            return context.QmsNatureofaction.AsNoTracking().Where(n => n.DeletedAt == null);
        }

        public IQueryable<QmsErrortype> RetrieveErrorTypes()
        {
            //return context.QmsErrortype.OrderBy(e => e.DisplayOrder);
            return context.QmsErrortype.AsNoTracking().Where(e => e.DeletedAt == null).OrderBy(e => e.Description);
        }

        public IQueryable<QmsCorrectiveactiontype> RetrieveActionTypes()
        {
            return context.QmsCorrectiveactiontype.AsNoTracking().Where(a => a.DeletedAt == null);
        }


        public IQueryable<QmsStatusTrans> RetrieveStatusTransitionTypes(int fromStatusID)
        {
            return context.QmsStatusTrans.AsNoTracking().Where(s => s.FromStatusId == fromStatusID && s.DeletedAt == null).Include(s => s.ToStatus);
        }
        
        public Status GetStatus(string statusCode)
        {
            var retval = (from statuses in context.QmsStatus.AsNoTracking()
                         where statuses.StatusCode == statusCode && statuses.DeletedAt == null
                         select new Status{
                             StatusId = statuses.StatusId,
                             StatusCode = statuses.StatusCode,
                             StatusLabel = statuses.StatusLabel,
                             DisplayOrder = statuses.DisplayOrder
                         }).SingleOrDefault();

            return retval;
        }

        public List<QmsStatusTrans> RetrieveStatusTransitionTypes(int orgId, int fromStatusId)
        {
            return RetrieveStatusTransitionTypes(orgId,fromStatusId,WorkItemTypeEnum.CorrectiveActionRequest);
        }

        public List<QmsStatusTrans> RetrieveStatusTransitionTypes(int orgId, int fromStatusId, string workItemType)
        {            
            List<QmsStatusTrans> retval = new List<QmsStatusTrans>();
            var orgstatustransitions = context.QmsOrgStatusTrans.AsNoTracking().Where(o => o.FromOrgId == orgId && o.DeletedAt == null && o.WorkItemTypeCode == workItemType).Include(o => o.ToOrgtype).ToList();
            foreach(var orgstatustransition in orgstatustransitions)
            {
                try
                {
                    var orgTypeCode = orgstatustransition.ToOrgtype.OrgtypeCode;
                    var statustransition = context.QmsStatusTrans.AsNoTracking().Where(s => s.FromStatusId == fromStatusId && s.StatusTransId == orgstatustransition.StatusTransId && s.DeletedAt == null).Include(s => s.ToStatus).SingleOrDefault();
                    var  statustranslabel = statustransition.StatusTransLabel;
                    statustransition.StatusTransLabel = statustranslabel;
                    retval.Add(statustransition);                    
                }
                catch (System.Exception)
                {
                }

            }
            return retval;            
        }

        public StatusTransition RetrieveOrgStatusTranstion(int qmsStatusTransId)
        {
            QmsStatusTrans statusTrans = context.QmsStatusTrans.AsNoTracking().Where(s => s.StatusTransId == qmsStatusTransId && s.DeletedAt == null).Include(s => s.ToStatus).Include(s => s.FromStatus).SingleOrDefault();
            QmsStatus fromStatus = statusTrans.FromStatus;
            QmsStatus toStatus = statusTrans.ToStatus;
            StatusTransition retval = new StatusTransition(statusTrans,fromStatus,toStatus);
            return retval;
        }

        public IQueryable<SecOrg> RetrieveServiceCenters()
        {
            return context.SecOrg.AsNoTracking().Where(o => o.DeletedAt == null && o.OrgtypeId == 10);
        }


        public NtfEmaillog RetrieveEmailLog(string logDate)
        {
            return context.NtfEmaillog.AsNoTracking().Where(l => l.SentDate == logDate).SingleOrDefault();
        }

        internal int SaveEmailLog(NtfEmaillog emaillog)
        {
            this.context.Add(emaillog);
            return context.SaveChanges();
        }

        internal QmsPersonnelOfficeIdentifier RetrievePoidByOrgCode(int orgId)
        {
            return context.QmsPersonnelOfficeIdentifier.Where(p => p.OrgId == orgId).SingleOrDefault();
        }

    }
}