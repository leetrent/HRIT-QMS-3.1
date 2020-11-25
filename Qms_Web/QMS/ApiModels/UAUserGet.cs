using System.Text;

namespace QMS.ApiModels
{
    public class UAUserGet
    {
        public string UserId { get; set; }
        public string OrgId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayLabel { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UAUserGet = {");
            sb.Append("UserId: ");
            sb.Append(this.UserId);
            sb.Append(", OrgId: ");
            sb.Append(this.OrgId);
            sb.Append(", EmailAddress: ");
            sb.Append(this.EmailAddress);
            sb.Append(", DisplayLabel: ");
            sb.Append(this.DisplayLabel);
            sb.Append("}");
            return sb.ToString();
        }
    }
}

