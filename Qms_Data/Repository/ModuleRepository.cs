using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class ModuleRepository
    {
        private QMSContext context;
        public ModuleRepository()
        {
            context = new QMSContext();
        }

        public ModuleRepository(QMSContext qMSContext)
        {
            context = qMSContext;
        }

        public IQueryable<SysModule> RetrieveAll()
        {
            return context.SysModule.AsNoTracking();
        }

        public int Create(SysModule newModule)
        {
            context.SysModule.Add(newModule);
            context.SaveChanges();
            return newModule.ModuleId;
        }

        public SysModule RetrieveById(int moduleId)
        {
            return context.SysModule.Where(m => m.ModuleId == moduleId).SingleOrDefault();
        }

        public void UpdateModule(SysModule module)
        {
            SysModule oldModule = RetrieveById(module.ModuleId);

            context.Entry(oldModule).State = EntityState.Deleted;
            context.Entry(module).State = EntityState.Modified;
            module.UpdatedAt = DateTime.Now;        
            context.SaveChanges();    
        }        






    }//end class
}//end namespace
