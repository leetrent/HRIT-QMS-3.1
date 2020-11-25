using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QmsCore
{
    public sealed class Data : QmsCore.Model.QMSContext
    {
        private static readonly Data context = new Data();

        public static Data Context
        {
            get{return context;}
        }

        private Data() : base()
        {
        }

    }//end class
}//end namespace