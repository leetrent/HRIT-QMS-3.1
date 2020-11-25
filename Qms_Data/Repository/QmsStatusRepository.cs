using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public class QmsStatusRepository
    {
        QMSContext context;

        public QmsStatusRepository()
        {
            context = new QMSContext();
        }

        public QmsStatus RetrieveByCode(string statusCode)
        {
            return context.QmsStatus.AsNoTracking().Where(s => s.StatusCode == statusCode).SingleOrDefault();
        }

        public List<QmsStatus> RetrieveAll()
        {
            return context.QmsStatus.AsNoTracking().ToList();
        }
    }
}