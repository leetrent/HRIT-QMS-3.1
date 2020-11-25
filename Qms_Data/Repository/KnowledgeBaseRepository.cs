using System;
using System.Linq;
using System.Collections.Generic;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;

namespace QmsCore.Repository
{
    public class KnowledgeBaseRepository 
    {
        QMSContext context;

        public KnowledgeBaseRepository()
        {
            context = new QMSContext();
        }

        public List<QmsKnowledgebase> RetrieveByType(string errorType)
        {
            return context.QmsKnowledgebase.AsNoTracking().Where(k => k.ErrorType == errorType).ToList();
        }

        public QmsKnowledgebase RetrieveByTypeAndCode(string errorType, string errorCode)
        {
            return context.QmsKnowledgebase.AsNoTracking().Where(k => k.ErrorType == errorType && k.ErrorCode == errorCode).SingleOrDefault();
        }

        public QmsKnowledgebase RetrieveById(int id)
        {
            return context.QmsKnowledgebase.Where(k => k.ItemId == id).SingleOrDefault();
        }

        public void Insert(QmsKnowledgebase entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }        

        public void Update(QmsKnowledgebase entity)
        {
            QmsKnowledgebase oldEntity = this.RetrieveById(entity.ItemId);
            update(oldEntity,entity);

        }

        internal void update(QmsKnowledgebase oldEntity, QmsKnowledgebase newEntity)
        {
            context.Entry(oldEntity).State = EntityState.Deleted;
            context.Entry(newEntity).State = EntityState.Modified;
        }     
        public void Delete(QmsKnowledgebase entity)
        {
            entity.DeletedAt = DateTime.Now;
            Update(entity);
        }

        public void Delete(QmsKnowledgebase entity, bool doHardDelete)
        {
            if(doHardDelete)
            {
                QmsKnowledgebase oldEntity = this.RetrieveById(entity.ItemId);
                context.Remove(oldEntity);
            }
            else
            {
                entity.DeletedAt = DateTime.Now;
                Update(entity);
            }

        }






    }//end class
}//end namespace