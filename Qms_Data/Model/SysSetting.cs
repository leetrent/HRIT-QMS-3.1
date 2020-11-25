using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public partial class SysSetting
    {
        public int SettingId { get; set; }
        public int SettingTypeId { get; set; }
        public string SettingValue { get; set; }
        public string Environment { get; set; }

        public SysSettingtype SettingType { get; set; }
    }
}
