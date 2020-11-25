using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Model;
using QmsCore.UIModel;
using QmsCore.Repository;
using QmsCore.QmsException;
using System.Linq;

namespace QmsCore.Services
{
    public class ModuleService
    {
        private ModuleRepository moduleRepository;

        public ModuleService()
        {
            moduleRepository = new ModuleRepository();
        }

        public ModuleService(QMSContext qMSContext)
        {
            moduleRepository = new ModuleRepository(qMSContext);
        }

        public Module RetrieveById(int moduleId)
        {
            return new Module(moduleRepository.RetrieveById(moduleId));
        }

        public List<Module> RetrieveAll()
        {
            List<Module> retval = new List<Module>();
            var modules = moduleRepository.RetrieveAll();
            foreach(var module in modules)
            {
                retval.Add(new Module(module));
            }
            return retval;
        }


        public List<Module> RetrieveAllActive()
        {
            List<Module> retval = new List<Module>();
            var modules = moduleRepository.RetrieveAll().Where(m => m.DeletedAt == null);
            foreach(var module in modules)
            {
                retval.Add(new Module(module));
            }
            return retval;
        }

        public List<Module> RetrieveAllArchived()
        {
            List<Module> retval = new List<Module>();
            var modules = moduleRepository.RetrieveAll().Where(m => m.DeletedAt != null);
            foreach(var module in modules)
            {
                retval.Add(new Module(module));
            }
            return retval;
        }



        

    }//end class
}//end namespace