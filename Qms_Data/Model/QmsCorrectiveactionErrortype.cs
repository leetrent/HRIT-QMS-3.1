using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsCorrectiveactionErrortype
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public int ErrorTypeId { get; set; }

        public QmsCorrectiveactionrequest CorrectiveAction { get; set; }
        public QmsErrortype ErrorType { get; set; }
    }
}
