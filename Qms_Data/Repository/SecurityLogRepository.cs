using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using QmsCore.Model;


namespace QmsCore.Repository
{
    internal class SecurityLogRepository
    {
        internal QMSContext context;

        internal SecurityLogRepository()
        {
            context = new QMSContext();
        }

        internal SecurityLogRepository(QMSContext qMSContext)
        {
            context = qMSContext;
        }

        internal void SaveEntry(SecSecuritylog logEntry)
        {
            context.SecSecuritylog.Add(logEntry);
            context.SaveChanges();
        }

        internal SecSecuritylogtype RetrieveSecurityLogType(string securityLogTypeCode)
        {
            return context.SecSecuritylogtype.Where(s => s.SecurityLogTypeCode == securityLogTypeCode).SingleOrDefault();
        }

        



    }//end class
}//end namespace