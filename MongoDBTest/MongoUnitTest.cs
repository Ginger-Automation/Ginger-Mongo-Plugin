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

using GingerMongoDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB;
using System.Collections.Generic;
using System.Data;

namespace MongoDBTest
{

    [TestClass]
    public class MongoUnitTest
    {
        public static MongoDbConnection db = new MongoDbConnection();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            db.ConnectionString = "mongodb://localhost:27017/";
        }


        [TestMethod]
        public void ExecuteQueryEmptyFilter()
        {
            //Arrange
            string query = "db.students.find({})";

            //Act
            List<string> documents = db.ExecuteQuery(query);   // get all records

            //Assert
            Assert.AreEqual("{ \"_id\" : ObjectId(\"5dc21f9a0992bc1ad8c33c13\"), \"First\" : \"Jojo\", \"Last\" : \"Momo\" }", documents[0]);
            Assert.AreEqual("{ \"_id\" : ObjectId(\"5dc21fa80992bc1ad8c33c14\"), \"First\" : \"Dina\" }", documents[1]);
            Assert.AreEqual(2, documents.Count);
            
        }

        [TestMethod]
        public void ExecuteQueryWithFilter()
        {
            //Arrange
            string query = "db.students.find({First: \"Dina\"})";

            //Act
            List<string> documents = db.ExecuteQuery(query);   

            //Assert            
            Assert.AreEqual("{ \"_id\" : ObjectId(\"5dc21fa80992bc1ad8c33c14\"), \"First\" : \"Dina\" }", documents[0]);
            Assert.AreEqual(1, documents.Count);

        }

        [TestMethod]
        public void ExecuteQueryWithFilterAndFields()
        {
            //Arrange
            string query = "db.students.find({First: \"Dina\"}, {First})";

            //Act
            List<string> documents = db.ExecuteQuery(query);   

            //Assert
            Assert.AreEqual("{ \"_id\" : ObjectId(\"5dc21f9a0992bc1ad8c33c13\"), \"First\" : \"Jojo\", \"Last\" : \"Momo\" }", documents[0]);
            Assert.AreEqual("{ \"_id\" : ObjectId(\"5dc21fa80992bc1ad8c33c14\"), \"First\" : \"Dina\" }", documents[1]);
            Assert.AreEqual(2, documents.Count);

        }


        [TestMethod]
        public void SyntaxParserTest()
        {
            // Arrange
            SyntaxParser1 syntaxParser1 = new SyntaxParser1();            

            // Act
            var result = syntaxParser1.Tokenize("db.students.find({First: \"Dina\"})");
            // CommandExecutor commandExecutor = db.GetCommandExecutor();

            List<string> rc= db.CommandExecutor.RunDBAction(result);            
        }



        //[TestMethod]
        //public void GetTableList()
        //{
        //    //Arrange
        //    List<string> Tables = null;

        //    //Act
        //    Tables = db.GetTablesList();

        //    //Assert
        //    Assert.AreEqual(1, Tables.Count);
        //    Assert.AreEqual("mycollection", Tables[0]);
        //}

        [TestMethod]
        public void GetTablesColumns()
        {
            //Arrange
            List<string> Columns = null;
            string tablename = "mycollection";

            //Act
            Columns = db.GetTablesColumns(tablename);

            //Assert
            Assert.AreEqual(3, Columns.Count);
            Assert.AreEqual("name", Columns[0]);
            Assert.AreEqual("age", Columns[1]);
            Assert.AreEqual("website", Columns[2]);
        }

        //[TestMethod]
        //public void RunUpdateCommand()
        //{
        //    //Arrange
        //    string upadateCommand = "db.mycollection.update({ \"name\" :  \"ff\"},{$set: { \"website\" : \"aaa.com\"} }); ";
        //    string result = null;

        //    //Act
        //    result = db.RunUpdateCommand(upadateCommand, false);

        //    //Assert
        //    Assert.AreEqual(result, "Success");
        //}

        [TestMethod]
        public void GetSingleValue()
        {
            //Arrange
            string result = null;

            //Act
            result = db.GetSingleValue("mycollection", "age", "20");
            if (result.Contains("zzz"))
            {
                result = "zzz";
            }
            //Assert
            Assert.AreEqual(result, "zzz");
        }

        //[TestMethod]
        //public void DBQuery()
        //{
        //    //Arrange
        //    DataTable result = null;

        //    //Act
        //    result = db.DBQuery("db.students.count");

        //    //Assert
        //    Assert.AreEqual(result.Rows.Count, 16);
        //}

        //[TestMethod]
        //public void GetRecordCount()
        //{
        //    //Arrange
        //    int a = 0;

        //    //Act
        //    a = db.GetRecordCount("mycollection");

        //    //Assert
        //    Assert.AreEqual(a, 16);
        //}



    }
}
