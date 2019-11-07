using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GingerMongoDB.SyntaxParser
{
    public class CommandExecutor
    {        
        IMongoDatabase mDB;

        public CommandExecutor(IMongoDatabase mDB)
        {           
            this.mDB = mDB;
        }

        
        public List<string> RunDBAction(IEnumerable<DslToken> tokens)
        {
            string sTokens = "";
            List<DslToken> kk = new List<DslToken>();
            foreach (DslToken t1 in tokens)
            {
                sTokens += SyntaxParser1.GetKey(t1.tokenType);

                // fix collection name  - remove db.****.  keep only collection name
                if (t1.tokenType == TokenType.CollectionName)
                {
                    t1.value = t1.value.Substring(3, t1.value.Length - 4);
                }
                kk.Add(t1);
            }


            List<string> result = null;
            // Do routing to the relevant method
            switch (sTokens)
            {
                case  "dbcol.find(json)":
                    result = Find(kk);
                    break;
                case "dbcol.find(json,json)":
                    result =  FindWithFields(kk);
                    break;
                default:
                    // err !!!!!!!!!!!!!
                    break;
            }
            return result;
            
        }

        

        private List<string> FindWithFields(List<DslToken> tokens)
        {
            throw new NotImplementedException();
        }

        private List<string> Find(List<DslToken> tokens)
        {
            string collection = tokens[0].value;
            string filter = tokens[3].value;
            var rec = mDB.GetCollection<BsonDocument>(collection).Find(filter);

            List<string> result = new List<string>();
            foreach (var d in rec.ToList())
            {
                result.Add(d.ToJson());
            }

            return result;
        }

        

    }
}
