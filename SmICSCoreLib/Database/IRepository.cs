using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public interface IRepository<T> where T : class
    {
        //checked
        IEnumerable<T> FindAll();
        void Insert(T item);
        void Delete(T item);

        //not checked 
        void DeleteByID(T item, string attribute, string id);
        void Update(T item);
        T FindOne(int id);
    }
}
