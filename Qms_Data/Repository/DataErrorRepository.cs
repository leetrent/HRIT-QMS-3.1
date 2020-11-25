using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.UIModel;

namespace QmsCore.Repository
{ 
    public class DataErrorRepository: BaseRepository<QmsCore.Model.QmsDataerror>
    {

        public DataErrorRepository(QMSContext qMSContext)
        {
            this.context = qMSContext;
        }

        public DataErrorRepository() : base()
        {

        }

        public override void Update(QmsDataerror entity)
        {
            QmsDataerror oldEntity = this.RetrieveById(entity.DataErrorId);
            base.update(oldEntity,entity);
            base.Save();

        } 


        public override QmsDataerror RetrieveById(int id) 
        {
            return context.QmsDataerror.AsNoTracking().Where(e => e.DataErrorId == id)
                                        .Include(e => e.AssignedToOrg)
                                        .Include(e => e.AssignedToUser)
                                        .Include(e => e.CreatedByOrg)
                                        .Include(e => e.CreatedByUser)
                                        .Include(e => e.Empl)
                                        .Include(e => e.Status)
                                        .Include(e => e.ErrorList).ThenInclude(l => l.DataItem)
                                        .SingleOrDefault();
        }

        public override IQueryable<QmsDataerror> RetrieveAll()
        {
            return context.QmsDataerror.AsNoTracking().Where(e => e.DeletedAt == null)
                                                      .Include(e => e.AssignedToOrg)
                                                      .Include(e => e.AssignedToUser)
                                                      .Include(e => e.CreatedByUser)
                                                      .Include(e => e.CreatedByOrg)
                                                      .Include(e => e.Empl)
                                                      .Include(e => e.Status)
                                                      .Include(e => e.ErrorList).ThenInclude(l => l.DataItem);
        }

        internal QmsWorkitemcomment RetrieveLatestComment(int entityId)
        {
            return context.QmsWorkitemcomment.Where(c => c.WorkItemId == entityId && c.WorkItemTypeCode == WorkItemTypeEnum.EHRI).Include(c => c.Author).OrderByDescending(c => c.WorkItemId).FirstOrDefault();
        }


    }//end class
}//end namespace