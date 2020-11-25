using System;

namespace QmsCore.QmsException
{
    public class OrganizationHasNoOrgTypeException : System.Exception
    {
        public OrganizationHasNoOrgTypeException() { }
        public OrganizationHasNoOrgTypeException(string message) : base(message) { }
        public OrganizationHasNoOrgTypeException(string message, System.Exception inner) : base(message, inner) { }
        protected OrganizationHasNoOrgTypeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}