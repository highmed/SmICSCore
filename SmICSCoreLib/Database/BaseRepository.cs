using Cassandra;
using Cassandra.Mapping;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Database
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private Cluster Cluster { get; set; }
        private ISession Session { get; set; }
        private IMapper Mapper { get; set; }

        public BaseRepository(string url, string keyspace)
        {
            //build cluster connection
            Cluster = Cluster.Builder().AddContactPoint(url).Build();
            Session = Cluster.Connect();

            //prepare schema
            Session.Execute(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS " + keyspace + " WITH replication = { 'class': 'SimpleStrategy', 'replication_factor': '1' };"));
            Session.Execute(new SimpleStatement("USE " + keyspace + " ;"));

            //create table BlAttribute just for testing 
            Session.Execute(new SimpleStatement("create table BlAttribute (BlAttributeId int primary key, Bundesland varchar, FallzahlGesamt varchar, Faelle7BL varchar," +
                                                     " FaellePro100000Ew varchar, Todesfaelle varchar, Todesfaelle7BL varchar, Inzidenz7Tage varchar, Farbe varchar); "));

            //create an instance of a Mapper from the session
            Mapper = new Mapper(Session);
        }

        public IEnumerable<T> FindAll()
        {
            try
            {
                return Mapper.Fetch<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void Insert(T item)
        {
            try
            {
                Mapper.Insert(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Delete(T item)
        {
            try
            {
                Mapper.Delete(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Update(T item)
        {
            try
            {
                Mapper.Update(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public T FindByID(string attribute, int id)
        {
            try
            {
                return Mapper.SingleOrDefault<T>("WHERE " + attribute + " = ?", id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public void DeleteByID(string attribute, int id)
        {
            try
            {
                var obj = FindByID(attribute, id);

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
        public void UpdateByID(T item, string attribute, int id)
        {
            try
            {
                var obj = FindByID(attribute, id);

                if (obj != null)
                {
                    DeleteByID(attribute, id);
                    Insert(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
