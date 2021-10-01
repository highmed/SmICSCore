using SmICSCoreLib.StatistikDataModels;
using System;

namespace SmICSCoreLib.Database
{
    public class DBService : IDBService
    {
        public void Save(Object obj, RepositoryType type)
        {
            if (type == RepositoryType.BUNDESLANDNEW)
            {
                UnitOfWork<BundeslandNew> unitOfWork = new();
                try
                {
                    unitOfWork.Repository.Insert((BundeslandNew)obj);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public Object Find(string attribute, string id, RepositoryType type)
        {
            if (type == RepositoryType.BUNDESLANDNEW)
            {
                UnitOfWork<BundeslandNew> unitOfWork = new();
                try
                {
                    return unitOfWork.Repository.FindByAttribute(attribute, id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
