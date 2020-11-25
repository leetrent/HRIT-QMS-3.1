using System;

namespace QmsCore.QmsException
{
    public class LockingException : System.Exception
    {
        public LockingException() { }
        public LockingException(string message) : base(message) { }
        public LockingException(string message, System.Exception inner) : base(message, inner) { }
        protected LockingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}