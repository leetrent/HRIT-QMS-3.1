using System;

namespace QmsCore.QmsException
{
    public class InvalidStatusTypeException : System.Exception
    {
        public InvalidStatusTypeException() { }
        public InvalidStatusTypeException(string message) : base(message) { }
        public InvalidStatusTypeException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidStatusTypeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}