using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using QmsCore.Model;
 
namespace QmsCore.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IAssignable
    {


        internal QMSContext context = null;
        internal DbSet<T> table = null;

        public BaseRepository()
        {
            context = new QMSContext();
            table = context.Set<T>();
        }

        public virtual T RetrieveById(int id)
        {
            return table.Find(id);
        }              

        public virtual IQueryable<T> RetrieveAll()
        {
            return table;
        }


        public virtual void Insert(T entity)
        {
            throw new NotImplementedException("Entity insert method has not been defined.");
        }

        public virtual void Delete(T entity)
        {
            throw new NotImplementedException("Entity deletion method has not been defined.");
        }

        public virtual void Delete(T entity, bool doHardDelete)
        {
            throw new NotImplementedException("Entity deletion method has not been defined.");
        }
        public virtual void Update(T entity)
        {
            throw new NotImplementedException("Entity update method has not been defined.");
        }

        internal void update(T oldEntity, T newEntity)
        {
            context.Entry(oldEntity).State = EntityState.Deleted;
            context.Entry(newEntity).State = EntityState.Modified;
            newEntity.UpdatedAt = DateTime.Now;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}//end namespace