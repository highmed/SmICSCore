using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public interface IDBService
    {
        void Insert<T>(T obj) where T : class;
        void Delete<T>(T obj) where T : class;
        void Update<T>(T obj) where T : class;
        IEnumerable<T> FindAll<T>() where T : class;
        IEnumerable<T> FindAllByAttribute<T>(string attribute, string value) where T : class;
        T FindOneByAttribute<T>(string attribute, string value) where T : class;
        void DeleteByAttribute<T>( string attribute, string value) where T : class;
        void UpdateByAttribute<T>(T obj, string attribute, string value) where T : class;

    }
}
