#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion


using Amdocs.Ginger.Plugin.Core.DatabaseLib;
using GingerMongoDB;
using GingerMongoDB.SyntaxParser;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MongoDB
{
    public class MongoDbConnection : IDatabase , INoSQLDatabase
    {
        MongoClient mMongoClient = null;
        string DbName;
        
        
        string User = null;
        string Password = null;
        string TNS = null;
        private DateTime LastConnectionUsedTime;
        string collectionName = "";
        
        string Database { get; set; }
        

        private string mConnectionString = null;
        public string ConnectionString
        {
            get
            {
                return mConnectionString;
            }
            set
            {
                mConnectionString = value;
            }
        }

        IMongoDatabase mDB
        {
            get
            {
                MongoClient mongoClient = new MongoClient(ConnectionString);
                Database = "Col1";  /// !!!!!!!!!!!!!!!!!! temp
                IMongoDatabase db = mongoClient.GetDatabase(Database);
                return db;
            }
        }

        public CommandExecutor CommandExecutor 
        { 
            get 
            {
                // TODO: no need for new !!!!!!!!!!!!!! cache
                CommandExecutor commandExecutor = new CommandExecutor(mDB);
                return commandExecutor;
            }  
        }

        public List<string> ExecuteQuery(string query)
        {            
            SyntaxParser1 syntaxParser = new SyntaxParser1();
            var tokens = syntaxParser.Tokenize(query);
            CommandExecutor commandExecutor = new CommandExecutor(mDB);
            List<string>  result = commandExecutor.RunDBAction(tokens);
            return result;            
        }

        
        

        
        //public DataTable DBQuery(string Query)
        //{
        //    collectionName = GetCollectionName(Query);
        //    var DB = mMongoClient.GetDatabase(DbName);
        //    DataTable dt = new DataTable();
        //    var collection = DB.GetCollection<BsonDocument>(collectionName);
        //    List<object> list = new List<object>();
        //    if (Query.Contains("insertMany"))
        //    {
        //        string queryParam = GetUpdateQueryParams(Query).ToString();
        //        Newtonsoft.Json.Linq.JArray jsonArray = Newtonsoft.Json.Linq.JArray.Parse(queryParam);
        //        List<BsonDocument> documents = new List<BsonDocument>();
        //        foreach (Newtonsoft.Json.Linq.JObject obj in jsonArray.Children<Newtonsoft.Json.Linq.JObject>())
        //        {
        //            BsonDocument document = BsonDocument.Parse(obj.ToString());
        //            documents.Add(document);
        //        }
        //        collection.InsertMany(documents);
        //    }
        //    else if (Query.Contains("insertOne") || Query.Contains("insert"))
        //    {
        //        BsonDocument insertDocumnet = BsonDocument.Parse(GetUpdateQueryParams(Query));
        //        collection.InsertOne(insertDocumnet);
        //    }
        //    else
        //    {
        //        var result = collection.Find(GetQueryParamater(Query, "find")).
        //        Project(Builders<BsonDocument>.Projection.Exclude("_id").Exclude(GetQueryParamater(Query, "projection"))).
        //        Sort(BsonDocument.Parse(GetQueryParamater(Query, "sort"))).
        //        Limit(Convert.ToInt32(GetQueryParamater(Query, "limit"))).
        //        ToList();

        //        var json = result.ToJson();
        //        dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

        //        //Act.ParseJSONToOutputValues(obj.ToString(), 1);
        //    }

        //    return dt;
        //}

        //[Obsolete]
        //public int GetRecordCount(string TName)
        //{
        //    MakeSureConnectionIsOpen();
        //    collectionName = TName;
        //    var DB = mMongoClient.GetDatabase(DbName);
        //    var collection = DB.GetCollection<BsonDocument>(collectionName);
        //    var count = collection.Count(new BsonDocument());
        //    return Convert.ToInt32(count.ToString());
        //}

        public string GetSingleValue(string Table, string Column, string Where)
        {
            string col = Column;
            string where = Where;
            string filter = "";
            var isNumeric = double.TryParse(where, out double n);

            collectionName = Table;
            var DB = mMongoClient.GetDatabase(DbName);
            var collection = DB.GetCollection<BsonDocument>(collectionName);
            //Simply matches on specific column type int
            //For ex where contains any int value
            if (isNumeric)
            {
                filter = "{" + col + ":" + where + "}";
            }
            else
            {
                //Equality matches on the whole embedded document require an exact match of the specified <value> document, including the field order
                //For ex where contains value = {field1:_value1,field2:_value2,field3:"_value3",...}
                if (where.Contains(","))
                {
                    filter = "{" + col + ":" + where + "}";
                }
                //Simply matches on specific column
                //For ex where contains any string value
                else
                {
                    filter = "{" + col + ":\"" + where + "\"}";
                }
            }
            var resultSimpleSQLOne = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList().ToJson();
            return resultSimpleSQLOne.ToString();
        }

        public List<string> GetTablesColumns(string collectionName)
        {
            
            List<string> columns = new List<string>();
            var db = mMongoClient.GetDatabase(DbName);
            var collection = db.GetCollection<BsonDocument>(collectionName);

            var result = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            Dictionary<string, object> dictionary = null;
            foreach (var row in result)
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(row.ToString());
                List<string> previousRowColumns = columns.ToList();
                var currentRowColumns = previousRowColumns.Union(dictionary.Keys);
                if (currentRowColumns.Count() > previousRowColumns.Count)
                {
                    columns.Clear();//clear previousRowColumns columns
                    foreach (string key in currentRowColumns)
                    {
                        columns.Add(key);
                    }
                }
            }

            return columns;
        }

        
        
        //public bool OpenConnection(Dictionary<string, string> parameters)
        //{
        //    return true;
        //    //KeyvalParamatersList = parameters;
        //    //GetConnectionString(parameters);
        //    //try
        //    //{
        //    //    ///
        //    //    /// ConnectionString format
        //    //    /// "mongodb://user1:password1@localhost/DB"
        //    //    ///
        //    //    if (ConnectionString != null && !string.IsNullOrEmpty(ConnectionString))
        //    //    {
        //    //        var connectionString = ConnectionString;
        //    //        DbName = MongoUrl.Create(ConnectionString).DatabaseName;
        //    //        if (DbName == null)
        //    //        {
        //    //            return false;
        //    //        }
        //    //        mMongoClient = new MongoClient(connectionString);
        //    //    }
        //    //    else
        //    //    {
        //    //        ///
        //    //        /// Host format
        //    //        /// "mongodb://HostOrIP:27017/DBName"
        //    //        ///
        //    //        string TNS = parameters.FirstOrDefault(pair => pair.Key == "TNS").Value;

        //    //        string[] HostPortDB = TNS.Split('/');

        //    //        string[] HostPort = HostPortDB[0].Split(':');

        //    //        //need to get db name

        //    //        MongoCredential mongoCredential = null;
        //    //        MongoClientSettings mongoClientSettings = null;
        //    //        if (HostPort.Length == 2 && HostPortDB.Length == 2)
        //    //        {
        //    //            if (string.IsNullOrEmpty(HostPortDB[HostPortDB.Length - 1]))
        //    //            {
        //    //                // mReporter.ToLog2(eLogLevel.ERROR, "Database is not mentioned in the TNS/Host.");
        //    //                return false;
        //    //            }

        //    //            if (string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(User))
        //    //            {
        //    //                mongoClientSettings = new MongoClientSettings
        //    //                {
        //    //                    Server = new MongoServerAddress(HostPort[0], Convert.ToInt32(HostPort[1]))
        //    //                };
        //    //            }
        //    //            else
        //    //            {
        //    //                bool res = false;
        //    //                String deCryptValue = "";// EncryptionHandler.DecryptString(Password, ref res, false);
        //    //                if (res == true)
        //    //                {
        //    //                    mongoCredential = MongoCredential.CreateCredential(HostPortDB[HostPortDB.Length - 1], User, deCryptValue);
        //    //                }
        //    //                else
        //    //                {
        //    //                    mongoCredential = MongoCredential.CreateCredential(HostPortDB[HostPortDB.Length - 1], User, Password);
        //    //                }
        //    //                mongoClientSettings = new MongoClientSettings
        //    //                {
        //    //                    Server = new MongoServerAddress(HostPort[0], Convert.ToInt32(HostPort[1])),
        //    //                    //UseSsl = true,
        //    //                    Credentials = new[] { mongoCredential }
        //    //                };
        //    //            }
        //    //            DbName = HostPortDB[HostPortDB.Length - 1];
        //    //            mMongoClient = new MongoClient(mongoClientSettings);
        //    //        }
        //    //        else
        //    //        {
        //    //            return false;
        //    //        }

        //    //    }
        //    //    //check dbname is present in the dblist
        //    //    if (GetDatabaseList().Contains(DbName))
        //    //    {
        //    //        return true;
        //    //    }
        //    //    else
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    // mReporter.ToLog2(eLogLevel.ERROR, "Failed to connect to Mongo DB", e);
        //    //    return false;
        //    //}
        //}


        public List<string> GetDatabaseList()
        {
            return mMongoClient.ListDatabaseNames().ToList();
        }

        //public string RunUpdateCommand(string updateCmd, bool commit = true)
        //{
        //    collectionName = GetCollectionName(updateCmd);
        //    var DB = mMongoClient.GetDatabase(DbName);
        //    var collection = DB.GetCollection<BsonDocument>(collectionName);
        //    //do commit
        //    if (commit)
        //    {
        //        var session = mMongoClient.StartSession();
        //        session.StartTransaction();
        //        UpdateCollection(updateCmd, collection);
        //        session.CommitTransaction();
        //    }
        //    else
        //    {
        //        UpdateCollection(updateCmd, collection);
        //    }
        //    return "Success";
        //}
        
        //private string GetUpdateQueryParams(string inputSQL)
        //{
        //    int startIndex = inputSQL.IndexOf("(") + 1;
        //    int endIndex = inputSQL.IndexOf(")");
        //    string updateQueryParams = inputSQL.Substring(startIndex, endIndex - startIndex);
        //    return updateQueryParams.Trim();
        //}
        //void UpdateCollection(string query, IMongoCollection<BsonDocument> collection)
        //{
        //    string updateQueryParams = GetUpdateQueryParams(query);
        //    var updateQueryParamsStrings = updateQueryParams.Split(new char[] { ',' }, 2);
        //    //set filter
        //    var filterString = updateQueryParamsStrings[0];
        //    //set param
        //    var paramString = updateQueryParamsStrings[1];
        //    BsonDocument filterDocumnet = BsonDocument.Parse(filterString);
        //    BsonDocument paramDocumnet = BsonDocument.Parse(paramString);

        //    if (query.Contains("updateMany"))
        //    {
        //        collection.UpdateMany(filterDocumnet, paramDocumnet);
        //    }
        //    else
        //    {
        //        collection.UpdateOne(filterDocumnet, paramDocumnet);
        //    }
        //}
        //public List<string> GetTablesList(string Name = null)
        //{
            
        //    if (string.IsNullOrEmpty(Name))
        //    {
        //        Name = this.DbName;
        //    }
        //    List<string> table = new List<string>();
        //    var db = mMongoClient.GetDatabase(Name);
        //    var names = db.ListCollectionNames().ToList();
        //    foreach (var item in names)
        //    {
        //        table.Add(item.ToString());
        //    }
        //    return table;
        //}

        //public bool TestConnection()
        //{
        //    throw new NotImplementedException();
        //}

        
    }
}
