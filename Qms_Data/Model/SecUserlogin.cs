using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SecUserlogin
    {
        public int Id { get; set; }
        public string Emailaddress { get; set; }
        public string LoginEventType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
