using QmsCore.UIModel;

namespace QMS.Extensions
{
    public static class PermissionExtension
    {
        public static string CodeAndValue(this Permission permission)
        {
            return $"permission.PermissionCode ({permission.PermissionLabel}";
        }
    }
}
    
