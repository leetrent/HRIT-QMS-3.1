using System;
using QmsCore.Model;
using System.Linq;

namespace QmsCore.Repository
{
    public interface IUserRepository
    {
        SecUser RetrieveByEmailAddress(string emailAddress);
    }
}