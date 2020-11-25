using System.Text;

namespace QMS.ViewModels
{
    public class UARoleViewModel
    {
        public int      RoleId      { get; set; }
        public string   RoleCode    { get; set; }
        public string   RoleLabel   { get; set; }
        public bool     Selected { get; set; } = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UARoleViewModel = {");
            sb.Append("RoleId: ");
            sb.Append(this.RoleId);
            sb.Append(", RoleCode: ");
            sb.Append(this.RoleCode);
            sb.Append(", RoleLabel: ");
            sb.Append(this.RoleLabel);
            sb.Append(", Selected: ");
            sb.Append(this.Selected);
            sb.Append("}");
            return sb.ToString();
        }
    }
}