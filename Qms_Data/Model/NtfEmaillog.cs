using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class NtfEmaillog
    {
        public int EmailLogId { get; set; }
        public string SentDate { get; set; }
        public int SentAmount { get; set; }
    }
}
