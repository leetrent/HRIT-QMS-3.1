
using System;

using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Status : IComparable<Status>
    {
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusLabel { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        internal Status()
        {}

        public Status(QmsStatus status)
        {
            this.StatusId = status.StatusId;
            this.StatusCode = status.StatusCode;
            this.StatusLabel = status.StatusLabel;
            this.DisplayOrder = status.DisplayOrder;
            this.CreatedAt = status.CreatedAt;
            this.UpdatedAt = status.UpdatedAt;
            this.DeletedAt = status.DeletedAt;
        }

        public Status(QmsStatus status, string displayLabel, int statusId)
        {
            this.StatusId = statusId;
            this.StatusCode = status.StatusCode;
            this.StatusLabel = displayLabel;
            this.DisplayOrder = status.DisplayOrder;
            this.CreatedAt = status.CreatedAt;
            this.UpdatedAt = status.UpdatedAt;
            this.DeletedAt = status.DeletedAt;
        }        

        public int CompareTo(Status other)
        {
            return this.StatusLabel.CompareTo(other.StatusLabel);
        }
    }//end class
}//end namespace