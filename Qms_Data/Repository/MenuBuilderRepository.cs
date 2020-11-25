using System;
using System.Linq;
using QmsCore.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QmsCore.Repository
{
    public class MenuBuilderRepository
    {
        private QMSContext context;

        public MenuBuilderRepository()
        {
            context = new QMSContext();
        }

        public MenuBuilderRepository(QMSContext qMSContext)
        {
            context = qMSContext;
        }

        public IQueryable<SysModuleRole> RetrieveByRoleId(int roleId)
        {
            return context.SysModuleRole.AsNoTracking().Where(s => s.RoleId == roleId)
                                                       .Include(s => s.Module).ThenInclude(m => m.SysMenuitem)
                                                       .Include(s => s.Role).ThenInclude(r => r.SecRolePermission);

        }

    }
}