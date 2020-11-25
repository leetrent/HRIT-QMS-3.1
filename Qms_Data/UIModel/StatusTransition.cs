using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class StatusTransition
    {
        public Status FromStatus { get; set; }
        public Status ToStatus { get; set; }
        public string TransitionLabel { get; set; }

        public StatusTransition(QmsStatusTrans statusTrans, QmsStatus fromStatus, QmsStatus toStatus)
        {
            FromStatus = new Status(fromStatus);
            ToStatus = new Status(toStatus);
            TransitionLabel = statusTrans.StatusTransLabel;
        }
    }
}