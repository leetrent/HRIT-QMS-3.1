
using System.Text;
using QmsCore.UIModel;

namespace QMS.Extensions
{
    public static class CorrectiveActionExtension
    {
        public static string ToJson(this CorrectiveAction ca)
        {
            StringBuilder sb = new StringBuilder("CorrectiveAction = {");
            sb.Append("Id: ");
            sb.Append(ca.Id);
            sb.Append(", AssignedToUserId: ");
            sb.Append(ca.AssignedToUserId);
            sb.Append(", ActionRequestTypeId: ");
            sb.Append(ca.ActionRequestTypeId);
            sb.Append(", EmplId: ");
            sb.Append(ca.EmplId);
            sb.Append(", NOACode: ");
            sb.Append(ca.NOACode);

            sb.Append(", (NatureOfAction == null): ");
            sb.Append(ca.NatureOfAction == null);

            if ( ca.NatureOfAction != null)
            {
                sb.Append(", NatureOfAction.RoutesToBr: ");
                sb.Append(ca.NatureOfAction.RoutesToBr);             
            }
            
            sb.Append(", EffectiveDateOfPar: ");
            sb.Append(ca.EffectiveDateOfPar);
            sb.Append(", IsPaymentMismatch: ");
            sb.Append(ca.IsPaymentMismatch);
            sb.Append(", StatusId: ");
            sb.Append(ca.StatusId);
            sb.Append(", ActionId: ");
            sb.Append(ca.ActionId);

            sb.Append(", ErrorTypes = [");          
            if (ca.ErrorTypes != null)
            {
                int count = 0;
                foreach ( var errorType in ca.ErrorTypes)
                {
                    if (count > 0) { sb.Append(", "); }
                    sb.Append("ErrorType: {");
                    sb.Append(" errorType.Id: ");
                    sb.Append(errorType.Id);
                    sb.Append(", errorType.RoutesToBR: ");
                    sb.Append(errorType.RoutesToBR);
                    sb.Append(", errorType.Description: ");
                    sb.Append(errorType.Description);
                    sb.Append("}");
                }
             }
             sb.Append("]");
             sb.Append("}");
            return sb.ToString();
        }
    }
}
