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

        public BaseRepository(string url, string keyspace)
        {
            //build cluster connection
            Cluster = Cluster.Builder().AddContactPoint(url).Build();
            Session = Cluster.Connect();

            //prepare schema
            Session.Execute(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS " + keyspace + " WITH replication = { 'class': 'SimpleStrategy', 'replication_factor': '1' };"));
            Session.Execute(new SimpleStatement("USE " + keyspace + " ;"));

            //create Types and a Table to inter a DailyReport 

            Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS BlAttribute (Bundesland text, FallzahlGesamt text, Faelle7BL text,  FaellePro100000Ew text, Todesfaelle text," +
                                                "Todesfaelle7BL text, Inzidenz7Tage text, Farbe text);"));

            Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Landkreis (Stadt text, LandkreisName text, FallzahlGesamt text, Faelle7Lk text, FaellePro100000Ew text," +
                                                "Inzidenz7Tage text, Todesfaelle text, Todesfaelle7Lk text, AdmUnitId text);"));

            Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Bundesland (BlAttribute frozen<BlAttribute>, Landkreis set<frozen<Landkreis>> );"));

            Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Bericht (Stand text, StandAktuell boolean, Fallzahl text, FallzahlVortag text, Todesfaelle text," +
                                                "TodesfaelleVortag text, RWert7Tage text, RWert7TageVortag text, Inzidenz7Tage text, Inzidenz7TageVortag text, GesamtImpfung text," +
                                                "ImpfStatus boolean, ErstImpfung text, ZweitImpfung text, Bundesland set<frozen<Bundesland>>, BlStandAktuell boolean);"));

            Session.Execute(new SimpleStatement("create table IF NOT EXISTS DailyReport (Id int primary key, Bericht frozen<Bericht>);"));

            //add UserDefinedTypes 
            Session.UserDefinedTypes.Define(UdtMap.For<BlAttribute>());
            Session.UserDefinedTypes.Define(UdtMap.For<Landkreis>());
            Session.UserDefinedTypes.Define(UdtMap.For<Bundesland>());
            Session.UserDefinedTypes.Define(UdtMap.For<Bericht>());

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
