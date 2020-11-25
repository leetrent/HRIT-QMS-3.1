using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class ErrorType : IComparable<ErrorType>
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public string RoutesToBR { get; set; }     

        public ErrorType(QmsCorrectiveactionErrortype actionErrorType)
        {
            Console.WriteLine($"[ErrorType] => (actionErrorType == null)....: {actionErrorType == null}");
            Console.WriteLine($"[ErrorType] => (actionErrorType.ErrorTypeId): {actionErrorType.ErrorTypeId}");
            Console.WriteLine($"[ErrorType] => (actionErrorType.ErrorType)..: {actionErrorType.ErrorType == null}");

            this.RoutesToBR = actionErrorType.ErrorType.RoutesToBr;
            this.Description = actionErrorType.ErrorType.Description;
            this.Id = actionErrorType.ErrorTypeId;
        }


        internal ErrorType()
        {

        }

        int IComparable<ErrorType>.CompareTo(ErrorType other)
        {
            return this.Id.Value.CompareTo(other.Id.Value);
        }
    }
}