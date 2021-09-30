using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> FindAll();  
        void Insert(T Entity);
        void Delete(T Entity);
        void Update(T Entity);
        T FindByAttribute(string attribute, string value);
        void DeleteByAttribute(string attribute, string value);
        void UpdateByAttribute(T Entity, string attribute, string value);   
    }
}
