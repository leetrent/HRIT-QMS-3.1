using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QmsCore.Model
{
    public partial class QmsDataerror : IAssignable
    {
        [NotMapped]
        public int Id {get {return this.DataErrorId;} set{this.DataErrorId = value;}}


        [NotMapped]
        public string WorkItemType {
            get {
                return Model.WorkItemTypeEnum.EHRI;
            }
        }

    }//end class
}//end namespace