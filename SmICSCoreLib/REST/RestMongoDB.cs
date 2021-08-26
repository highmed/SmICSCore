using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.REST
{
    public class RestMongoDB
    {
        private IMongoDatabase _db;
        //public RestMongoDB db = new RestMongoDB("SmICSLocalStorage");
        public RestMongoDB(string database)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var dbList = client.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
            //var db = client.GetDatabase(database);
        }

        public List<T> LoadDocuments<T>(string document)
        {
            var collection = _db.GetCollection<T>(document);
            return collection.Find(new BsonDocument()).ToList();
        }

        public List<T> LoadDocumentByID<T>(string document, Guid id)
        {
            var collection = _db.GetCollection<T>(document);
            var filter = Builders<T>.Filter.Eq("ID", id);
            return collection.Find(filter).ToList();
        }

        public void InsertDocument<T>(string document, T doc)
        {

            if (_db.GetCollection<T>(document)==null)
            {
                _db.CreateCollection(document);
            }
            var collection = _db.GetCollection<T>(document);
            collection.InsertOne(doc);
            
        }

        [Obsolete]
        public void UpsertDocument<T>(string document, Guid id, T doc)
        {
            var collection = _db.GetCollection<T>(document);
            var result = collection.ReplaceOne(
                new BsonDocument("ID", id),
                doc,
                new UpdateOptions { IsUpsert = true });
        }

        public void DeleteDocument<T>(string document, Guid id)
        {
            var collection = _db.GetCollection<T>(document);
            var filter = Builders<T>.Filter.Eq("ID", id);
            collection.DeleteOne(filter);
        }
    }
}
