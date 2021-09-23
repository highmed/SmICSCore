using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> FindAll();
        T FindByID(string attribute, int id);
        void Insert(T item);
        void Delete(T item);
        void Update(T item);
        void DeleteByID(string attribute, int id);
        void UpdateByID(T item, string attribute, int id);   
    }
}
