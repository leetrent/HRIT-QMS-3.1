using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QmsCore.Model;
using QmsCore.Repository;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public class BaseService<T> where T :  IAssignable
    {
        internal QMSContext context = null;

        public BaseService()
        {
            context = new QMSContext();
        }
     







        internal void addHistory(IAssignable item, User actionTakenByUser,string ActionDescription)
        {
            QmsWorkitemhistory itemHistory = new QmsWorkitemhistory();
            itemHistory.ActionTakenByUserId = actionTakenByUser.UserId;
            itemHistory.CreatedAt = DateTime.Now;
            itemHistory.WorkItemId = item.Id;
            itemHistory.PreviousStatusId = item.StatusId;
            itemHistory.PreviousAssignedByUserId = item.AssignedByUserId;
            itemHistory.PreviousAssignedToOrgId = item.AssignedToOrgId;
            itemHistory.PreviousAssignedtoUserId = item.AssignedToUserId;
            itemHistory.ActionDescription = ActionDescription;
            itemHistory.WorkItemTypeCode = item.WorkItemType;
            context.Add(itemHistory);
            context.SaveChanges();
        }

        internal void addSearchHistory(IAssignable item, User searcher)
        {
            QmsWorkitemviewlog log = new QmsWorkitemviewlog();
            log.Createdat = DateTime.Now;
            log.Workitemid = item.Id;
            log.WorkItemTypeCode = item.WorkItemType;
            log.Userid = searcher.UserId;

            context.Add(log);
            context.SaveChanges();
            
        }


        public void AddComment(IAssignable item, User author, string message)
        {
            QmsWorkitemcomment comment = new QmsWorkitemcomment();
            comment.Message = message;
            comment.WorkItemId = item.Id;
            comment.WorkItemTypeCode = item.WorkItemType;
            comment.AuthorId = author.UserId;
            comment.CreatedAt = DateTime.Now;
            context.Add(comment);
            context.SaveChanges();        
        }
        

    }
}