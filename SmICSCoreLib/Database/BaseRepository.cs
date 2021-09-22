using Cassandra;
using Cassandra.Mapping;
using System;
using System.Collections.Generic;
using SmICSCoreLib.Test;
using System.Linq;

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
            //Session.Execute(new SimpleStatement("create table BlAttribute (BlAttributeId int primary key, Bundesland varchar, FallzahlGesamt varchar, Faelle7BL varchar," +
            //" FaellePro100000Ew varchar, Todesfaelle varchar, Todesfaelle7BL varchar, Inzidenz7Tage varchar, Farbe varchar); "));
            Session.Execute(new SimpleStatement("CREATE TYPE if not exists Adresse (street text, city text, zip int);"));
            Session.Execute(new SimpleStatement("CREATE TABLE if not exists TestKlasse (ID int primary key, Name text, Vorname text, Age int, Adresse set<frozen<Adresse>>);"));
            

            Session.UserDefinedTypes.Define(UdtMap.For<Adresse>());
            //create an instance of a Mapper from the session
            Mapper = new Mapper(Session);

            //var results = Session.Execute("SELECT ID, Name, Adresse FROM TestKlasse where id = '12345B';");
            //var row = results.First();
            // You retrieve the field as a value of type Address
            //var userAddress = row.GetValue<Adresse>("Adresse");
            //Console.WriteLine("The user lives on {0} Street", userAddress.Street);
        }

        //checked 
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

        //not checked 
        public void DeleteByID(T item, string attribute, string id)
        {
            try
            {
                Mapper.Delete<T>("WHERE " + attribute + " = " + id + " \" ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public T FindOne(int id)
        {
            throw new NotImplementedException();
        }  
        public void Update(T item)
        {
            throw new NotImplementedException();
        }

    }
}
