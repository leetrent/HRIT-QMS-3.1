
using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class WorkItemFile
    {
        public int? Id { get; set; }
        public int? UploadedByUserId { get; set; }
        public int WorkItemId { get; set; }
        public string WorkItemType { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public WorkItemFile(QmsWorkitemfile file)
        {
            this.Id = file.Id;
            this.UploadedByUserId = file.UploadedByUserId;
            this.WorkItemId = file.WorkItemId;
            this.WorkItemType = file.WorkItemTypeCode;
            this.FilePath = file.Filepath;
            this.FileType = file.Filetype;
            this.CreatedAt = file.Createdat;
            this.DeletedAt = file.Deletedat;
        }

        internal WorkItemFile()
        {



        }

        public QmsWorkitemfile QmsWorkitemfile()
        {
            QmsWorkitemfile wif = new QmsWorkitemfile();
            if(this.Id.HasValue)  
                wif.Id = this.Id.Value;

            wif.UploadedByUserId = this.UploadedByUserId;
            wif.WorkItemId = this.WorkItemId;
            wif.WorkItemTypeCode = this.WorkItemType;
            wif.Filepath = this.FilePath;
            wif.Filetype = this.FileType;
            wif.Createdat = this.CreatedAt;
            wif.Deletedat = this.DeletedAt;         



            return wif;
        }




    }
}