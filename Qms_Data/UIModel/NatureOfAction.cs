using System;
using System.Text;

namespace QmsCore.UIModel
{
    public class NatureOfAction
    {
        public string NoaCode { get; set; }
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }   

        public string RoutesToBr { get; set; }

        
        public string SelectOptionText
        {
            get { return $"{this.NoaCode} - {this.ShortDescription}"; }
        }

        public NatureOfAction()
        {}

        public NatureOfAction(QmsCore.Model.QmsNatureofaction noa)
        {
            this.NoaCode = noa.Noacode;
            this.LongDescription = noa.Description;
            this.ShortDescription = noa.ShortDescription;
            this.RoutesToBr = noa.RoutesToBr;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("NatureOfAction = {");
            sb.Append("NoaCode: ");
            sb.Append(NoaCode);
            sb.Append(", ShortDescription: ");
            sb.Append(ShortDescription);
            sb.Append(", LongDescription: ");
            sb.Append(LongDescription);
            sb.Append("}");
            return sb.ToString();
        }    
    }
}