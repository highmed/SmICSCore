using System;
namespace SmICSCoreLib.Database
{
    public interface IDBService
    {
        public void Save(Object obj, RepositoryType type);

        public Object Find(string attribute, string id, RepositoryType type);

        //public void Delete(Object obj, RepositoryType type);
    }
}
