
using System;
using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public partial class Module : IComparable<Module>
    {
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleLabel { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string QueryString { get; set; }
        public byte DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Module()
        {
            
        }

        public Module(SysModule sysModule)
        {
            this.DisplayOrder = sysModule.DisplayOrder;
            this.ModuleCode = sysModule.ModuleCode;
            this.ModuleLabel = sysModule.ModuleLabel;
            this.ActionName = sysModule.ActionName;
            this.ControllerName = sysModule.ControllerName;
            this.ModuleId = sysModule.ModuleId;
            this.CreatedAt = sysModule.CreatedAt;
            this.DeletedAt = sysModule.DeletedAt;
            this.UpdatedAt = sysModule.UpdatedAt;
            this.QueryString = sysModule.QueryString;
        }

        public ModuleMenuItem ModuleMenuItem()
        {
            ModuleMenuItem moduleMenuItem = new ModuleMenuItem();
            moduleMenuItem.DisplayOrder = this.DisplayOrder;
            moduleMenuItem.ModuleMenuItemId = this.ModuleId;
            moduleMenuItem.Title = this.ModuleLabel;
            moduleMenuItem.Controller = this.ControllerName;
            moduleMenuItem.ControllerAction = this.ActionName;
            moduleMenuItem.UseCase = this.QueryString;
            return moduleMenuItem;
        }

        public int CompareTo(Module other)
        {
            return this.DisplayOrder.CompareTo(other.DisplayOrder);
        }
    }//end class
}//end namespace