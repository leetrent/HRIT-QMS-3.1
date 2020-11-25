using System;

namespace QMS.ViewModels
{
    public class ErrorViewModel
    {
        public bool ShowErrorDetails { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public Exception QmsException { get; set; }
    }
}