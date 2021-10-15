using Cassandra;
using Cassandra.Mapping;
using SmICSCoreLib.StatistikDataModels;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private Cluster Cluster { get; set; }
        private ISession Session { get; set; }
        private IMapper Mapper { get; set; }

        public BaseRepository(string url, string keyspace, string user, string password)
        {
            //build cluster connection
            Cluster = Cluster.Builder().AddContactPoint(url).WithCredentials(user, password).Build();
            Session = Cluster.Connect();

            //prepare schema
            //Session.Execute(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS " + keyspace + " WITH replication = { 'class': 'SimpleStrategy', 'replication_factor': '1' };"));
            Session.Execute(new SimpleStatement("USE " + keyspace + " ;"));
      
            //create an instance of a Mapper from the session
            Mapper = new Mapper(Session);
        }
     
        public void Insert(T Entity)
        {
            try
            {
                Mapper.Insert(Entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Delete(T Entity)
        {
            try
            {
                Mapper.Delete(Entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Update(T Entity)
        {
            try
            {
                Mapper.Update(Entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public IEnumerable<T> FindAll()
        {
            try
            {
                return Mapper.Fetch<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public IEnumerable<T> FindAllByAttribute(string attribute, string value)
        {
            try
            {
                return Mapper.Fetch<T>("WHERE " + attribute + " = '" + value + "' ALLOW FILTERING");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public T FindOneByAttribute(string attribute, string value)
        {
            try
            {
                return Mapper.SingleOrDefault<T>("WHERE " + attribute + " = ?", value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public void DeleteByAttribute(string attribute, string value)
        {
            try
            {
                var obj = FindOneByAttribute(attribute, value);
                if (obj != null)
                {
                    Delete(obj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void UpdateByAttribute(T newObj, string attribute, string value)
        {
            try
            {
                var oldObj = FindOneByAttribute(attribute, value);
                if (oldObj != null)
                {
                    Delete(oldObj);
                    Insert(newObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
