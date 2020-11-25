using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.UIModel;

namespace QmsCore.Repository
{
    public class CorrectiveActionRepository : BaseRepository<QmsCore.Model.QmsCorrectiveactionrequest>, ICorrectiveActionRepository
    {

        public override void Update(QmsCorrectiveactionrequest entity)
        {
            QmsCorrectiveactionrequest oldEntity = this.RetrieveById(entity.Id);
            base.update(oldEntity,entity);
            base.Save();
        }

        public CorrectiveActionRepository(QMSContext qmsContext)
        {
            this.context = qmsContext;
        }

        public CorrectiveActionRepository() : base()
        {

        }


        public override QmsCorrectiveactionrequest RetrieveById(int correctiveActionRequestId)
        {
            return context.QmsCorrectiveactionrequest.Where(c => c.Id == correctiveActionRequestId)
                                                     .Include(e => e.ActionRequestType)
                                                     .Include(e => e.Status)
                                                     .Include(e => e.CreatedByUser)
                                                     .Include(e => e.CreatedAtOrg)
                                                     .Include(e => e.NatureOfActionNavigation)
                                                     .Include(e => e.AssignedToOrg)
                                                     .Include(e => e.Empl)
                                                     .Include(e => e.AssignedToUser)
                                                     .Include(e => e.QmsCorrectiveactionErrortype)
                                                     .ThenInclude(i => i.ErrorType).SingleOrDefault();
        }

        public override IQueryable<QmsCorrectiveactionrequest> RetrieveAll()
        {
            return context.QmsCorrectiveactionrequest.AsNoTracking().Include(e => e.CreatedByUser)
                                                                    .Include(e => e.Status)
                                                                    .Include(e => e.NatureOfActionNavigation)
                                                                    .Include(e => e.ActionRequestType)
                                                                    .Include(e => e.AssignedToOrg)
                                                                    .Include(e => e.AssignedToUser)
                                                                    .Include(e => e.Empl);
//            return context.QmsCorrectiveactionrequest.AsNoTracking().Where(e => e.DeletedAt == null && e.ResolvedAt == null).Include(e => e.Status).Include(e => e.NatureOfActionNavigation).Include(e => e.ActionRequestType).Include(e => e.AssignedToOrg).Include(e => e.Empl);
        }
 
        public IQueryable<QmsCorrectiveactionrequest> RetrieveAllIncludingResolved()
        {
            return context.QmsCorrectiveactionrequest.AsNoTracking().Where(e => e.DeletedAt == null).Include(e => e.Status).Include(e => e.NatureOfActionNavigation).Include(e => e.ActionRequestType).Include(e => e.AssignedToOrg).Include(e => e.Empl);
        }


        public IQueryable<QmsCorrectiveactionrequest> RetrieveAllByAssignedToUser(int AssignedToUserid)
        {
            return context.QmsCorrectiveactionrequest.AsNoTracking().Where(e => e.AssignedToUserId == AssignedToUserid && e.DeletedAt == null &&  e.ResolvedAt == null).Include(e => e.Empl).Include(e => e.Status).Include(e => e.NatureOfActionNavigation).Include(e => e.ActionRequestType); 
        }        

        public IQueryable<QmsCorrectiveactionrequest> RetrieveAllByAssignedToOrg(int AssignedToOrgId)
        {
            return context.QmsCorrectiveactionrequest.AsNoTracking().Where(e => e.AssignedToOrgId == AssignedToOrgId && e.DeletedAt == null && e.ResolvedAt == null).Include(e => e.Empl).Include(e => e.Status).Include(e => e.NatureOfActionNavigation).Include(e => e.ActionRequestType); 
        }        
         
        public IQueryable<QmsCorrectiveactionrequest> RetrieveAllByCreatedBy(int CreatedByUserId)
        {
            return context.QmsCorrectiveactionrequest.AsNoTracking().Where(e => e.CreatedByUserId == CreatedByUserId && e.DeletedAt == null && e.ResolvedAt == null).Include(e => e.Empl).Include(e => e.Status).Include(e => e.NatureOfActionNavigation).Include(e => e.ActionRequestType); 
        }              

        internal QmsWorkitemcomment RetrieveLatestComment(int entityId)
        {
            return context.QmsWorkitemcomment.Where(c => c.WorkItemId == entityId && c.WorkItemTypeCode == WorkItemTypeEnum.CorrectiveActionRequest).Include(c => c.Author).OrderByDescending(c => c.WorkItemId).FirstOrDefault();
        }




    }
}