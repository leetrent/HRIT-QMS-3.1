using System;
using System.Collections.Generic;
using QmsCore.UIModel;

using System.Linq;
using QmsCore.Repository;
using QmsCore.QmsException;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;

namespace QmsCore.Services
{
    public class ReferenceService : IReferenceService
    {
        ReferenceRepository repository;


        public ReferenceService()
        {
            repository = new ReferenceRepository();

        }

        public ReferenceService(IReferenceRepository referenceRepository)
        {
            repository = (ReferenceRepository)referenceRepository;
        }

        public ReferenceService(QMSContext qmsContext)
        {
            repository = new ReferenceRepository(qmsContext);
        }      

        public IQueryable<NatureOfAction> RetrieveNatureOfActions()
        {
            var ret = from recs in repository.RetrieveNatureOfActions()
                      where recs.DeletedAt == null && recs.Noacode != "000"
                      select new NatureOfAction{
                          NoaCode = recs.Noacode,
                          LongDescription = recs.Description,
                          ShortDescription = recs.ShortDescription,
                          RoutesToBr = recs.RoutesToBr
                      };


            return ret;
        }

        public NatureOfAction RetrieveNatureOfAction(string noaCode)
        {
            var ret = repository.RetrieveNatureOfActions().Where(r => r.Noacode == noaCode).Single();
            NatureOfAction noa = new NatureOfAction(ret);
            return noa;
        }
        
        public IQueryable<ErrorType> RetrieveErrorTypes()
        {
            var ret = from recs in repository.RetrieveErrorTypes()
                      where recs.DeletedAt == null
                      select new ErrorType{ 
                        Id = recs.Id,
                        Description = recs.Description,
                        RoutesToBR = recs.RoutesToBr
                      };

            return ret;                      

        }

        public IQueryable<ActionType> RetrieveActionTypes()
        {
            var ret = from recs in repository.RetrieveActionTypes()
                      where recs.DeletedAt == null
                      select new ActionType{ 
                          Id = recs.Id, 
                          Label = recs.Label};

            return ret;
                      
        }

        public StatusTransition RetrieveStatusTransition(int qmsOrgStatusTransId)
        {
            return repository.RetrieveOrgStatusTranstion(qmsOrgStatusTransId);
        }

        public List<Status> RetrieveAvailableActionsList(string statusType, Organization org)
        {
            Status status = repository.GetStatus(statusType);
            return RetrieveAvailableActionsList(status.StatusId,org);
        }
        
        public List<Status> RetrieveAvailableActionsList(int statusId, Organization org)
        {
            List<Status> retval = new List<Status>();
            var recs = repository.RetrieveStatusTransitionTypes(org.OrgId,statusId,WorkItemTypeEnum.CorrectiveActionRequest);
            foreach(var rec in recs)
            {
                retval.Add(new Status(rec.ToStatus,rec.StatusTransLabel, rec.StatusTransId));
            }
            retval.Sort();
            return retval;
        }

        public List<Status> RetrieveAvailableActionsList(int statusId, Organization org,string workItemType)
        {
            List<Status> retval = new List<Status>();
            var recs = repository.RetrieveStatusTransitionTypes(org.OrgId,statusId,workItemType);
            foreach(var rec in recs)
            {
                retval.Add(new Status(rec.ToStatus,rec.StatusTransLabel, rec.StatusTransId));
            }
            retval.Sort();
            return retval;
        }

        public SecOrg RetrieveOrgByOrgCode(string orgCode)
        {
            return repository.context.SecOrg.AsNoTracking().Where(o => o.OrgCode == orgCode).SingleOrDefault();
        }        

        public List<Organization> RetrieveServiceCenters()
        {
            List<Organization> organizations = new List<Organization>();
            var orgs = repository.RetrieveServiceCenters();
            foreach(var org in orgs)
            {
                organizations.Add(new Organization(org));    
            }
            return organizations;
        }

        public Status RetrieveStatusByStatusCode(string statusCode)
        {
            var qs = repository.context.QmsStatus.AsNoTracking().Where(s => s.StatusCode == statusCode).SingleOrDefault();
            var retval = new Status(qs);
            return retval;
        }


        public EmailLog RetrieveEmailLogByDate(string logDate)
        {
            EmailLog emailLog = new EmailLog();
            var log = repository.RetrieveEmailLog(logDate);
            if(log != null)
            {
                emailLog = new EmailLog(log);
            }
            return emailLog;
        }

        public int SaveEmailLog(EmailLog emailLog)
        {
            NtfEmaillog ntfEmaillog = emailLog.NtfEmailLog();
            return repository.SaveEmailLog(ntfEmaillog);
        }

    }
}