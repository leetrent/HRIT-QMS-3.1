using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class QmsStatus
    {
        public QmsStatus()
        {
            QmsCorrectiveactionrequest = new HashSet<QmsCorrectiveactionrequest>();
            QmsDataerror = new HashSet<QmsDataerror>();
            QmsStatusTransFromStatus = new HashSet<QmsStatusTrans>();
            QmsStatusTransToStatus = new HashSet<QmsStatusTrans>();
            QmsWorkitemhistory = new HashSet<QmsWorkitemhistory>();
        }

        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusLabel { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<QmsCorrectiveactionrequest> QmsCorrectiveactionrequest { get; set; }
        public ICollection<QmsDataerror> QmsDataerror { get; set; }
        public ICollection<QmsStatusTrans> QmsStatusTransFromStatus { get; set; }
        public ICollection<QmsStatusTrans> QmsStatusTransToStatus { get; set; }
        public ICollection<QmsWorkitemhistory> QmsWorkitemhistory { get; set; }
    }
}
