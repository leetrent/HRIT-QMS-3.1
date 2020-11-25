

using System.Collections.Generic;
using QmsCore.Model;

namespace QmsCore.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(T entity, bool doHardDelete);
        void Save();
    }
}//end namespace