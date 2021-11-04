using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public class DBService : IDBService
    {
        public void Insert<T>(T obj) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                unitOfWork.Repository.Insert(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete<T>(T obj) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                unitOfWork.Repository.Delete(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Update<T>(T obj) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                unitOfWork.Repository.Update(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public IEnumerable<T> FindAll<T>() where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                return unitOfWork.Repository.FindAll();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public IEnumerable<T> FindAllByAttribute<T>(string attribute, string value) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                return unitOfWork.Repository.FindAllByAttribute(attribute, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public IEnumerable<T> FindAllByAttributes<T>(string firstAttribute, string firstValue, string secondAttribute, string secondValue) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                return unitOfWork.Repository.FindAllByAttributes(firstAttribute, firstValue, secondAttribute, secondValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public T FindOneByAttribute<T>(string attribute, string value) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                return unitOfWork.Repository.FindOneByAttribute(attribute, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }

        public T FindOneByAttributes<T>(string firstAttribute, string firstValue, string secondAttribute, string secondValue) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                return unitOfWork.Repository.FindOneByAttributes(firstAttribute, firstValue, secondAttribute, secondValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }

        public void DeleteByAttribute<T>(string attribute, string value) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                unitOfWork.Repository.DeleteByAttribute(attribute, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateByAttribute<T>(T newObj, string attribute, string value) where T : class
        {
            UnitOfWork<T> unitOfWork = new();
            try
            {
                unitOfWork.Repository.UpdateByAttribute(newObj, attribute, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
