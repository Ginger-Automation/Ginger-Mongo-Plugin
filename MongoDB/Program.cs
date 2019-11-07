using System;
using System.Collections.Generic;
using System.Text;
using Amdocs.Ginger.CoreNET.Drivers.CommunicationProtocol;
using Amdocs.Ginger.Plugin.Core;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GingerMongoDB
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.Title = "Ginger MongoDb Database plugin";


            string simplefind = "db.*.find({})";
            string simplefindfields = "db.{database}.find({filter},{fields})";

            

            Console.WriteLine("Starting MongoDb Database Plugin");

            string conn = "mongodb://localhost:27017/Col1";
            MongoClient mongoClient = new MongoClient(conn);
            IMongoDatabase db = mongoClient.GetDatabase("Col1");
            var collection =  db.GetCollection<BsonDocument>("aaa");


            

            //var builder = Builders<object>.Projection;
            //builder.Expression(u => new { u.name });

            //var doc = new BsonDocument
            //{
            //    {"First", BsonValue.Create("David") },
            //    {"Last", BsonValue.Create("123") }
            //};
            // collection.InsertOne(doc);

            // var filter = new BsonDocument("First", "David");


            //  var filter = "{}, {First:1, Last:1}";  // valid json string

            // string fields = "";

            //var doc = new BsonDocument
            //{
            //    {
            //         new BsonDocument {"First", BsonValue.Create("David") };
            //    }

            //};


            // db.aaa.find({filter}, {fields})

            var filter = "{First: 'David'}";  // valid json string            
            string fields = "{ First: 1, Last: 1}";

            // var rec = collection.Find(filter).Project(fields);

            var rec = collection.Find(filter);
               var r2 = rec.Project(fields);
            // var r3 = r2.Limit(2);

            // collection.Find()

            foreach (var d in rec.ToList())
            {
                Console.WriteLine(d);
            }

            // int count = collection.CountDocuments();

            



            //DbName = MongoUrl.Create(ConnectionString).DatabaseName;
            //if (DbName == null)
            //{
            //    return false;
            //}
            //mMongoClient = new MongoClient(connectionString);


            //using (GingerNodeStarter gingerNodeStarter = new GingerNodeStarter())
            //{
            //    if (args.Length > 0)
            //    {
            //        gingerNodeStarter.StartFromConfigFile(args[0]);  // file name 
            //    }
            //    else
            //    {
            //        gingerNodeStarter.StartNode("MSAccess Service 1", new MongoDbConnection(), SocketHelper.GetLocalHostIP(), 15001);
            //    }
            //    gingerNodeStarter.Listen();
            //}

        }

        // syntax "db.*collection*.find({filter},{fields})";   //fields optional
        void find(IMongoDatabase db, string collection, string filter, string fields = null)
        {            
            var col = db.GetCollection<BsonDocument>("aaa");
            col.Find(filter).Project(fields);

        }

    }
}
