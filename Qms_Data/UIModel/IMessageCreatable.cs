using System;

namespace QmsCore.UIModel
{
    public interface IMessageCreatable
    {
        string Template {get;}
        string Message {get;}
    }
}