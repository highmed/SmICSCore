using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public interface IRepository<T> where T : class
    {
         
        void Insert(T Entity);
        void Delete(T Entity);
        void Update(T Entity);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindAllByAttribute(string attribute, string value);
        T FindOneByAttribute(string attribute, string value);
        void DeleteByAttribute(string attribute, string value);
        void UpdateByAttribute(T Entity, string attribute, string value);   
    }
}
