using System;

namespace QmsCore.Engine
{
    public partial class Column : IComparable<Column>
    {
        public string TableFieldname {get;set;}
        public string DataType {get;set;}
        public int Index {get;set;}

        public int CompareTo(Column other)
        {
            return this.Index.CompareTo(other.Index);
        }
    }//end class
}//end namespace