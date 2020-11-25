using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsKnowledgebase
    {
        public int ItemId { get; set; }
        public string ErrorType { get; set; }
        public string ErrorCode { get; set; }
        public string ShortDescription { get; set; }
        public string ErrorDescription { get; set; }
        public string Impact { get; set; }
        public string Risk { get; set; }
        public string CorrectionComplexity { get; set; }
        public string SupportingDocLink { get; set; }
        public string Comment { get; set; }
        public string Hrsupport { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
