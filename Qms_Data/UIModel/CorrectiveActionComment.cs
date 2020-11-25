using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class CorrectiveActionComment
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public string Message { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string WorkItemType {get {return Model.WorkItemTypeEnum.CorrectiveActionRequest;}}

        public User Author { get; set; }

        public CorrectiveActionComment(string message
                                      ,int correctiveActionId
                                      ,int authorId)
        {
            this.CorrectiveActionId = correctiveActionId;
            this.AuthorId = authorId;
            this.Message = message;
            this.CreatedAt = DateTime.Now;

        }

        public CorrectiveActionComment(QmsWorkitemcomment comment, bool enableUserSecurityLoading)
        {
            this.Id = comment.Id;
            this.CorrectiveActionId = comment.WorkItemId.Value;
            this.Message = comment.Message;
            this.CreatedAt = comment.CreatedAt;
            this.AuthorId = comment.AuthorId.Value;
            this.Author = new User(comment.Author,enableUserSecurityLoading);
        }

        public QmsWorkitemcomment WorkItemComment()
        {
            QmsWorkitemcomment workItemComment = new QmsWorkitemcomment();
            workItemComment.Id = this.Id;
            workItemComment.WorkItemId = this.CorrectiveActionId;
            workItemComment.AuthorId = this.AuthorId;
            workItemComment.CreatedAt = this.CreatedAt;
            workItemComment.Message = this.Message;
            workItemComment.WorkItemTypeCode = Model.WorkItemTypeEnum.CorrectiveActionRequest;
            return workItemComment;
        }



    }//end class
}//end namespace