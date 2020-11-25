using System.Text;
using QmsCore.UIModel;

namespace QMS.Extensions
{
    public static class RoleExtensions
    {
        public static string ToMasterHtml(this Role role)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n\t\t\t<a class=\"list-group-item list-group-item-action\" ");
            sb.Append($"id=\"master-role-{role.RoleId}\" ");
            sb.Append("data-toggle=\"list\" ");
            sb.Append($"href=\"#detail-role-{role.RoleId}\" ");
            sb.Append("role=\"tab\" ");
            sb.Append($"aria-controls=\"role-{role.RoleId}\"");
            sb.Append(">");
            sb.Append(role.RoleCode);
            sb.Append("</a>\n");
            return sb.ToString();
        }

        public static string ToDetailHtml(this Role role)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n\t\t<div class=\"tab-pane fade\" ");
            sb.Append($"id=\"detail-role-{role.RoleId}\" ");
            sb.Append("role=\"tabpanel\" ");
            sb.Append($"aria-labelledby=\"master-role-{role.RoleId}\"");
            sb.Append(">");
            //sb.Append($"Permissions for Role {role.RoleCode}");
            sb.Append("\n\t\t\t<ul class=\"list-group\">");
            foreach (var permission in role.Permissions)
            {
                sb.Append("\n\t\t\t\t<li class=\"list-group-item\">");
                sb.Append(permission.PermissionCode);
                sb.Append("</li>");
            }
            sb.Append("\n\t\t\t</ul>");
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}