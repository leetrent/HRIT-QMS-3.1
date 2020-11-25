
using System;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class ModuleMenuItem :IComparable<ModuleMenuItem>
    {
        public int DisplayOrder {get;set;}
        public int ModuleMenuItemId {get;set;}
        public string Title {get;set;}
        public string Controller {get;set;}
        public string ControllerAction {get;set;}
        public string UseCase {get;set;}        

        public List<MenuItem> MenuItems{get;set;}

        public ModuleMenuItem()
        {
            MenuItems = new List<MenuItem>();
            UseCase = string.Empty;
        }

        public int CompareTo(ModuleMenuItem other)
        {
            return this.DisplayOrder.CompareTo(other.DisplayOrder);
        }
    }//end class
}//end namespace