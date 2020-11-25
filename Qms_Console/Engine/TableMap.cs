using System;
using System.Collections.Generic;

namespace QmsCore.Engine
{
    public partial class TableMap
    {
        public List<Column> Columns {get;set;}

        public string TableName {get;set;}

        public string SourceType {get;set;}

        public string ModelName {get;set;}

        public string Delimiter {get;set;}

        public bool HasHeader {get;set;}

        public bool UseSoftDelete {get;set;}

        public string CSVFileName {get;set;}

        public string Validator {get;set;}

    }//end class
}//end namespace