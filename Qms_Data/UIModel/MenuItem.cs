using System;
using System.Text;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class MenuItem : IComparable<MenuItem>
    {
        public int DisplayOrder {get;set;}
        public int MenuItemId {get;set;}
        public string Title {get;set;}
        public string Controller {get;set;}
        public string ControllerAction {get;set;}
        public string UseCase {get;set;}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MenuItem = { ");
            sb.Append("Title: ");
            sb.Append(this.Title);
            sb.Append(", Controller: ");
            sb.Append(this.Controller);
            sb.Append(", ControllerAction: ");
            sb.Append(this.ControllerAction);
            sb.Append(", UseCase: ");
            sb.Append(this.UseCase);
            sb.Append("}");
            return sb.ToString();
        }

        public int CompareTo(MenuItem other)
        {
            return this.DisplayOrder.CompareTo(other.DisplayOrder);
        }

        public MenuItem()
        {}

        public MenuItem(SysMenuitem sysMenuitem)
        {
            this.MenuItemId = sysMenuitem.MenuitemId;
            this.Controller = sysMenuitem.ControllerName;
            this.ControllerAction = sysMenuitem.ActionName;
            this.UseCase = sysMenuitem.QueryString;
            this.Title = sysMenuitem.MenuitemLabel;
            this.DisplayOrder = sysMenuitem.DisplayOrder;
        }
        
    }//end class
}//end namespace