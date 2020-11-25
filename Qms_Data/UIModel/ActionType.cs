using System;

namespace QmsCore.UIModel
{
    public class ActionType
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public string Code { get; set; }

        public  ActionType()
        {}

        public ActionType(QmsCore.Model.QmsCorrectiveactiontype at)
        {
            this.Id = at.Id;
            this.Label = at.Label;
            this.Code = at.Code;

        }
 
    }
}