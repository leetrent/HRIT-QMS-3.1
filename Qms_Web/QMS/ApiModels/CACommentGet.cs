using System.Text;

namespace QMS.ApiModels
{
    public class CACommentGet
    {
        public string OrgLabel { get; set; }
        public string DisplayName { get; set; }
        public string Message { get; set; }
        public string DateCreated { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CACommentGet = {");
            sb.Append("DisplayName: ");
            sb.Append(this.DateCreated);
            sb.Append(", OrgLabel: ");
            sb.Append(this.OrgLabel);
            sb.Append(", DateCreated: ");
            sb.Append(this.DateCreated);
            sb.Append(", Message: ");
            sb.Append(this.Message);
            sb.Append("}");
            return sb.ToString();
        }
    }
}

