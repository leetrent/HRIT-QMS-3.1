using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QmsCore.Model
{
    public partial class QmsEmployee
    {

        [NotMapped]
        public string Ssn { get; set; }        


    }//end class
}//end namespace