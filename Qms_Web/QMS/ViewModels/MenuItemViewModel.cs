using System.Text;

namespace QMS.ViewModels
{
    public class MenuItemViewModel
    {
        public int    MenuItemId        { get; set; }
        public int    CategoryId         { get; set; }
        public string MenuLabel         { get; set; }
        public string PermissionCode    { get; set; }
        public string ControllerName    { get; set; }
        public string ActionName        { get; set; }
        public string UseCase           { get; set; }
       
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;

            MenuItemViewModel other = (MenuItemViewModel) obj;
            return (this.CategoryId == other.CategoryId);
        }

        public override int GetHashCode()
        {
            return CategoryId;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MenuBuilderViewModel = {");
            sb.Append("MenuItemId: ");
            sb.Append(this.MenuItemId);
            sb.Append("CategoryId: ");
            sb.Append(this.CategoryId);
            sb.Append(", MenuLabel: ");
            sb.Append(this.MenuLabel);
            sb.Append(", PermissionCode: ");
            sb.Append(this.PermissionCode);
            sb.Append(", ControllerName: ");
            sb.Append(this.ControllerName);
            sb.Append(", ActionName: ");
            sb.Append(this.ActionName);
            sb.Append(", UseCase: ");
            sb.Append(this.UseCase);
            sb.Append("}");
            return sb.ToString();
        }
    }
}