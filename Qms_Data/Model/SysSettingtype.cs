using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SysSettingtype
    {
        public SysSettingtype()
        {
            SysSetting = new HashSet<SysSetting>();
        }

        public int SettingTypeId { get; set; }
        public string SettingCode { get; set; }
        public string SettingDescription { get; set; }
        public DateTime Createdat { get; set; }
        public DateTime? Deletedat { get; set; }

        public ICollection<SysSetting> SysSetting { get; set; }
    }
}
