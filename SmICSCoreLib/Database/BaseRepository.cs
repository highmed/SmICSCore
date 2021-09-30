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

            //first implementation
            ////create Types and a Table for Database

            //Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS BlAttribute (Bundesland text, FallzahlGesamt text, Faelle7BL text,  FaellePro100000Ew text, Todesfaelle text," +
            //                                    "Todesfaelle7BL text, Inzidenz7Tage text, Farbe text);"));

            //Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Landkreis (Stadt text, LandkreisName text, FallzahlGesamt text, Faelle7Lk text, FaellePro100000Ew text," +
            //                                    "Inzidenz7Tage text, Todesfaelle text, Todesfaelle7Lk text, AdmUnitId text);"));

            //Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Bundesland (BlAttribute frozen<BlAttribute>, Landkreis set<frozen<Landkreis>> );"));

            //Session.Execute(new SimpleStatement("CREATE type IF NOT EXISTS Bericht (Stand text, StandAktuell boolean, Fallzahl text, FallzahlVortag text, Todesfaelle text," +
            //                                    "TodesfaelleVortag text, RWert7Tage text, RWert7TageVortag text, Inzidenz7Tage text, Inzidenz7TageVortag text, GesamtImpfung text," +
            //                                    "ImpfStatus boolean, ErstImpfung text, ZweitImpfung text, Bundesland set<frozen<Bundesland>>, BlStandAktuell boolean);"));

            //Session.Execute(new SimpleStatement("create table IF NOT EXISTS DailyReport (Id int primary key, Bericht frozen<Bericht>);"));

            ////add UserDefinedTypes 
            //Session.UserDefinedTypes.Define(UdtMap.For<BlAttribute>());
            //Session.UserDefinedTypes.Define(UdtMap.For<Landkreis>());
            //Session.UserDefinedTypes.Define(UdtMap.For<Bundesland>());
            //Session.UserDefinedTypes.Define(UdtMap.For<Bericht>());

            //second implementation
            //create Tables for Database 

            Session.Execute(new SimpleStatement("CREATE table IF NOT EXISTS LandkreisNew (Id text primary key, Stadt text, LandkreisName text, FallzahlGesamt text, Faelle7Lk text, FaellePro100000Ew text," +
                                                "Inzidenz7Tage text, Todesfaelle text, Todesfaelle7Lk text, AdmUnitId text);"));

            Session.Execute(new SimpleStatement("CREATE table IF NOT EXISTS BundeslandNew (Id text primary key, Bundesland text, FallzahlGesamt text, Faelle7BL text,  FaellePro100000Ew text, Todesfaelle text," +
                                                "Todesfaelle7BL text, Inzidenz7Tage text, Farbe text);"));

            Session.Execute(new SimpleStatement("CREATE table IF NOT EXISTS BerichtNew (Id text primary key, Stand text, StandAktuell boolean, Fallzahl text, FallzahlVortag text, Todesfaelle text," +
                                               "TodesfaelleVortag text, RWert7Tage text, RWert7TageVortag text, Inzidenz7Tage text, Inzidenz7TageVortag text, GesamtImpfung text," +
                                               "ImpfStatus boolean, ErstImpfung text, ZweitImpfung text, BlStandAktuell boolean);"));


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
        public T FindByAttribute(string attribute, string value)
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
                var obj = FindByAttribute(attribute, value);

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
        public void UpdateByAttribute(T Entity, string attribute, string value)
        {
            try
            {
                var obj = FindByAttribute(attribute, value);

                if (obj != null)
                {
                    DeleteByAttribute(attribute, value);
                    Insert(Entity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
