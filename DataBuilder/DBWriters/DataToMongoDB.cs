using CldrToDatabases.DBModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CldrToDatabases.DataBuilder
{
    internal class DataToMongoDB : DataToDBs
    {
        public DataToMongoDB(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) :
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
        }

        public override bool UseDbContext()
        {
            return false;
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: 3.0.7
            // Using: Robomongo 0.8.5
            // No Password is set, because Robomongo can not connect to DB including a Password.

            // Connect to Server including Database.
            string connectionString = ConfigurationManager.ConnectionStrings[_DBType].ConnectionString;
            CreateMongoDatabase(connectionString);
        }

        public override void SaveDBData(DataDB dataDB)
        {
            // MongoDB doen't support foreign keys and no triggers!
            // The logic or foreign keys have to be done within the logic using the db-objects.
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == not defined

                string connectionString = ConfigurationManager.ConnectionStrings[_DBType].ConnectionString;
                MongoUrl mongoUrl = new MongoUrl(connectionString);
                string mongoDbName = mongoUrl.DatabaseName; // "Countries"
                MongoClient mongoDbClient = new MongoClient(mongoUrl);
                IMongoDatabase mongoDb = mongoDbClient.GetDatabase(mongoDbName);
                SaveToMongoDB(mongoDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveDBData on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", _TableToInsert, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        public override void CompareDBDataToLists(DataDB dataDB)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_DBType].ConnectionString;
            MongoUrl mongoUrl = new MongoUrl(connectionString);
            string mongoDbName = mongoUrl.DatabaseName; // "Countries"
            MongoClient mongoDbClient = new MongoClient(mongoUrl);
            IMongoDatabase mongoDb = mongoDbClient.GetDatabase(mongoDbName);

            // Compare Languages
            CompareListToMongoDB("Languages", _LngCount, _DataExtractor.LanguagesList, mongoDb);
            // Compare Scripts
            CompareListToMongoDB("Scripts", _ScrCount, _DataExtractor.ScriptsList, mongoDb);
            // Compare Territories
            CompareListToMongoDB("Territories", _TerCount, _DataExtractor.TerritoriesList, mongoDb);
            // Compare Currencies
            CompareListToMongoDB("Currencies", _CurCount, _DataExtractor.CurrenciesList, mongoDb);
            // Compare TimeZones
            CompareListToMongoDB("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, mongoDb);
            // Compare MetaZones
            CompareListToMongoDB("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, mongoDb);
            // Compare WindowsZones
            CompareListToMongoDB("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, mongoDb);

            // Locale Data

            // Compare LanguageNames
            CompareListToMongoDB("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, mongoDb);
            // Compare ScriptNames
            CompareListToMongoDB("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, mongoDb);
            // Compare TerritoryNames
            CompareListToMongoDB("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, mongoDb);
            // Compare CurrencyNames
            CompareListToMongoDB("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, mongoDb);
            // Compare TimeZoneNames
            CompareListToMongoDB("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, mongoDb);
            // Compare MetaZoneNames
            CompareListToMongoDB("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, mongoDb);
        }

        private void CreateMongoDatabase(string connectionString)
        {
            Console.WriteLine("Start create MongoDB Database.");
            try
            {
                MongoUrl mongoUrl = new MongoUrl(connectionString);
                string mongoDbName = mongoUrl.DatabaseName; // "Countries"
                MongoClient mongoDbClient = new MongoClient(mongoUrl);
                mongoDbClient.DropDatabase(mongoDbName);
                // DB will be created automatically when inserting first collection.
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CreateMongoDatabase ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToMongoDB(IMongoDatabase mongoDb)
        {
            // Save Languages
            SaveListToMongoDB("Languages", _LngCount, _DataExtractor.LanguagesList, mongoDb);
            // Save Scripts
            SaveListToMongoDB("Scripts", _ScrCount, _DataExtractor.ScriptsList, mongoDb);
            // Save Territories
            SaveListToMongoDB("Territories", _TerCount, _DataExtractor.TerritoriesList, mongoDb);
            // Save Currencies
            SaveListToMongoDB("Currencies", _CurCount, _DataExtractor.CurrenciesList, mongoDb);
            // Save TimeZones
            SaveListToMongoDB("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, mongoDb);
            // Save MetaZones
            SaveListToMongoDB("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, mongoDb);
            // Save WindowsZones
            SaveListToMongoDB("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, mongoDb);

            // Locale Data

            // Save LanguageNames
            SaveListToMongoDB("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, mongoDb);
            // Save ScriptNames
            SaveListToMongoDB("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, mongoDb);
            // Save TerritoryNames
            SaveListToMongoDB("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, mongoDb);
            // Save CurrencyNames
            SaveListToMongoDB("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, mongoDb);
            // Save TimeZoneNames
            SaveListToMongoDB("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, mongoDb);
            // Save MetaZoneNames
            SaveListToMongoDB("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, mongoDb);
        }

        private void SaveListToMongoDB<T>(string tableName, int rowCount, IList<T> list, IMongoDatabase mongoDb)
        {
            // When you insert an object to MongoDB:
            // If it doesn't have an "_id" field then the "_id" field will be added automatically by MongoDB to your document!
            // Or the driver adds one and sets it to a 12-byte MongoDB ObjectId value.
            long rowsInserted = 0;
            Console.WriteLine("Start copy {0} to MongoDB.", tableName);

            try
            {
                // If "_id" is not in use, so do not need an index for it by first creating the collection having AutoIndexId set to false.
                //mongoDb.CreateCollection(tableName, new CreateCollectionOptions { AutoIndexId = false });

                IMongoCollection<T> coll = mongoDb.GetCollection<T>(tableName);

                CreateIndexes(coll, tableName, list.First());

                var collectionData = new List<WriteModel<T>>();
                foreach (var item in list)
                {
                    collectionData.Add(new InsertOneModel<T>(item));
                }
                var result = coll.BulkWrite(collectionData);
                rowsInserted = result.InsertedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveListToMongoDB on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }

            Console.WriteLine("\r{2}: Copied {0} rows from {1} to be done.", rowsInserted, rowCount, tableName);
        }

        private void CreateIndexes<T>(IMongoCollection<T> coll, string tableName, T item)
        {
            Dictionary<DBModelsMongoDbIndexAttribute, string> indexColumns = GetIndexColumns(item);
            List<string> columnNames = null;
            BsonDocument bsonIndex = null;
            int currentIndex = -1;
            bool isUnique = false;
            foreach (var index in indexColumns.OrderBy(o => o.Key.IndexNumber).ThenBy(o => o.Key.FieldOrder))
            {
                if (currentIndex != index.Key.IndexNumber)
                {
                    if (bsonIndex != null)
                    {
                        CreateIndexOptions options = new CreateIndexOptions();
                        options.Unique = isUnique;
                        options.Name = tableName + "_" + string.Join("_", columnNames.ToArray());
                        coll.Indexes.CreateOne(bsonIndex, options);
                    }
                    currentIndex = index.Key.IndexNumber;
                    columnNames = new List<string>();
                    bsonIndex = new BsonDocument();
                }
                isUnique = index.Key.IsUnique;
                columnNames.Add(index.Value);
                bsonIndex.Add(index.Value, index.Key.SortOrder);
            }
            if (bsonIndex != null)
            {
                CreateIndexOptions options = new CreateIndexOptions();
                options.Unique = isUnique;
                options.Name = tableName + "_" + string.Join("_", columnNames.ToArray());
                coll.Indexes.CreateOne(bsonIndex, options);
            }
        }

        private Dictionary<DBModelsMongoDbIndexAttribute, string> GetIndexColumns<T>(T item)
        {
            PropertyInfo[] props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<DBModelsMongoDbIndexAttribute, string> indexColumns = new Dictionary<DBModelsMongoDbIndexAttribute, string>();
            foreach (var prop in props)
            {
                IEnumerable<DBModelsMongoDbIndexAttribute> attributes = (IEnumerable<DBModelsMongoDbIndexAttribute>)prop.GetCustomAttributes(typeof(DBModelsMongoDbIndexAttribute));
                if (attributes != null)
                {
                    foreach (var attr in attributes)
                    {
                        indexColumns.Add(attr, prop.Name);
                    }
                }
            }

            return indexColumns;
        }

        private void CompareListToMongoDB<T>(string tableName, int rowCount, IList<T> list, IMongoDatabase mongoDb)
        {
            string dbType = "MongoDB-Collection";
            long rowsInserted = 0;
            Console.WriteLine("Start Compare {0}-List to {1}.", tableName, dbType);

            try
            {
                IMongoCollection<T> coll = mongoDb.GetCollection<T>(tableName);
                var projection = Builders<T>.Projection.Exclude("_id");
                var collDocuments = coll.Find(new BsonDocument()).Project(projection).ToCursor().ToList();
                rowsInserted = collDocuments.Count();

                Console.WriteLine("\r{2}: '{0}' rows of '{1}' inserted.", rowsInserted, rowCount, tableName);
                // if not equal, always compare each row!
                if (rowsInserted != rowCount)
                {
                    List<T> listInDB = MongoCollectionToList<T>(collDocuments);
                    CompareDBToListDetails(listInDB, list, tableName, rowCount, dbType);
                }
                else if (_ForceFullCompare)
                {
                    List<T> listInDB = MongoCollectionToList<T>(collDocuments);
                    CompareDBToListDetails(listInDB, list, tableName, rowCount, dbType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CompareListToMongoDB on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private List<T> MongoCollectionToList<T>(List<BsonDocument> collDocuments)
        {
            List<T> listInDB = new List<T>();
            foreach (var document in collDocuments)
            {
                T item = BsonSerializer.Deserialize<T>(document);
                listInDB.Add(item);
            }

            return listInDB;
        }

        private void CompareDetailsRowByRow<T>(IMongoCollection<T> coll, IList<T> list, string tableName, int rowCount)
        {
            // Using Expression Trees
            // Is extremly slow!!
            Console.WriteLine("Start CompareDetailsRowByRow {0}-List to MongoDB-Collection.", tableName);

            try
            {
                List<string> keyColumns = GetKeyColumns(list.First());
                var projection = Builders<T>.Projection.Exclude("_id");
                var collDocuments = coll.Find(new BsonDocument()).Project(projection).ToCursor().ToList();

                IQueryable<T> queryableData = list.AsQueryable<T>();
                ParameterExpression pe = Expression.Parameter(typeof(T), tableName);
                Expression left;
                Expression right;
                int currentRow = 0;
                int notifyRow = 0;
                int notifyEach = 100;
                foreach (var document in collDocuments)
                {
                    List<Expression> exList = new List<Expression>();

                    T docElement = BsonSerializer.Deserialize<T>(document);
                    foreach (var keyColumn in keyColumns)
                    {
                        left = Expression.Property(pe, typeof(T).GetProperty(keyColumn));
                        right = Expression.Constant(document.GetValue(keyColumn).ToString());
                        exList.Add(Expression.Equal(left, right));
                    }
                    Expression exAll = exList.First();
                    int exListCount = exList.Count();
                    if (exListCount > 1)
                    {
                        for (int i = 1; i < exListCount; i++)
                        {
                            exAll = Expression.AndAlso(exAll, exList.ElementAt(i));
                        }
                    }
                    MethodCallExpression whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { queryableData.ElementType },
                        queryableData.Expression,
                        Expression.Lambda<Func<T, bool>>(exAll, new ParameterExpression[] { pe }));

                    IQueryable<T> results = queryableData.Provider.CreateQuery<T>(whereCallExpression);
                    if (results.Any())
                    {
                        int resultCount = results.Count();
                        if (resultCount > 1)
                            throw new Exception(string.Format("Document-Name {0} Multiple document-rows count='{1}':\r\n{2}.", tableName, resultCount.ToString(), document.ToString()));

                        if (!docElement.Equals(results.First()))
                            Console.WriteLine("Document-Name '{0}' document-row:\r\n{1}\r\nNot Equals:\r\n{2} .", tableName, docElement.ToString(), results.First().ToString());
                    }
                    else
                        Console.WriteLine("Document-Name '{0}' Not in List: document-row:\r\n{1}.", tableName, document.ToString());

                    currentRow++;
                    notifyRow++;
                    if (notifyRow == notifyEach)
                    {
                        Console.Write("\r{2}: Compared {0} rows from {1} to be done.", currentRow, rowCount, tableName);
                        notifyRow = 0;
                    }
                }
                Console.Write("\r{2}: Compared {0} rows from {1} to be done.", currentRow, rowCount, tableName);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CompareDetailsRowByRow on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}