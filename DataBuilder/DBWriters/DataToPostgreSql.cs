using CldrToDatabases.DBModels;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CldrToDatabases.DataBuilder
{
    internal class DataToPostgreSql : DataToDBs
    {
        public DataToPostgreSql(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) :
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: PostgreSQL 9.6.2 on x86_64-pc-mingw64, compiled by gcc.exe (Rev5, Built by MSYS2 project) 4.9.2, 64-bit
            // To make it work:
            // After install NuGet: Npgsql and EntityFramework6.Npgsql
            // Copy: Npgsql.dll and EntityFramework6.Npgsql.dll from each packages lib\net45 folder
            // To: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies
            // (Change to your Visual Studio version Program folder!)

            // Connect to Server including Database.
            // Else script doesn't work if the database doesn't exist!
            // Needed changes done in 'TestCreatePostgreSqlDB' to identify and maybe create the database.
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
            CreatePostgreSqlDBSchema(connectionString);
        }

        public override void SaveDBData(DataDB dataDB)
        {
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == "Npgsql"

                string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
                SaveToPostgreSql(connectionString, schemaName: "Countries");
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
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
            string schemaName = "Countries";

            // Compare Languages
            CompareListToPostgreSql("Languages", schemaName, _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Compare Scripts
            CompareListToPostgreSql("Scripts", schemaName, _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Compare Territories
            CompareListToPostgreSql("Territories", schemaName, _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Compare Currencies
            CompareListToPostgreSql("Currencies", schemaName, _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Compare TimeZones
            CompareListToPostgreSql("TimeZones", schemaName, _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Compare MetaZones
            CompareListToPostgreSql("MetaZones", schemaName, _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Compare WindowsZones
            CompareListToPostgreSql("WindowsZones", schemaName, _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Compare LanguageNames
            CompareListToPostgreSql("LanguageNames", schemaName, _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Compare ScriptNames
            CompareListToPostgreSql("ScriptNames", schemaName, _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Compare TerritoryNames
            CompareListToPostgreSql("TerritoryNames", schemaName, _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Compare CurrencyNames
            CompareListToPostgreSql("CurrencyNames", schemaName, _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Compare TimeZoneNames
            CompareListToPostgreSql("TimeZoneNames", schemaName, _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Compare MetaZoneNames
            CompareListToPostgreSql("MetaZoneNames", schemaName, _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void CreatePostgreSqlDBSchema(string connectionString)
        {
            Console.WriteLine("Start create PostgreSql DB and Schema.");
            try
            {
                TestCreatePostgreSqlDB(connectionString);

                RunPostgreSqlScript(connectionString, "PostgreSQL_Create_Schema.sql", "create Schema");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CreatePostgreSqlSchema ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void TestCreatePostgreSqlDB(string connectionString)
        {
            try
            {
                bool dbExists;
                // Connect to Database=postgres. (is PostgreSql default administrative database)
                // Else connection doesn't work if the database 'MyDB' doesn't exist!
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString.Replace(";Database='MyDB'", ";Database=postgres")))
                {
                    conn.Open();
                    string cmdText = "SELECT 1 FROM pg_database WHERE datname='MyDB'";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn))
                    {
                        dbExists = cmd.ExecuteScalar() != null;
                    }
                }
                if (!dbExists)
                {
                    // Connect only to Server, so remove the database from the existing connection string.
                    // Else connection doesn't work if the database doesn't exist!
                    RunPostgreSqlScript(connectionString.Replace(";Database='MyDB'", string.Empty), "PostgreSQL_Create_DB.sql", "create DB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-TestCreatePostgreSqlDB ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void RunPostgreSqlScript(string connectionString, string scriptFileName, string todo)
        {
            Console.WriteLine("Start {0} PostgreSql Database.", todo);
            try
            {
                FileInfo file = new FileInfo(Path.Combine(_ScriptsPath, scriptFileName));
                string scriptText = file.OpenText().ReadToEnd();
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    NpgsqlCommand command = new NpgsqlCommand(scriptText, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-RunPostgreSqlScript ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToPostgreSql(string connectionString, string schemaName)
        {
            // Save Languages
            SaveListToCsvFileToPostgreSql("Languages", schemaName, _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Save Scripts
            SaveListToCsvFileToPostgreSql("Scripts", schemaName, _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Save Territories
            SaveListToCsvFileToPostgreSql("Territories", schemaName, _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Save Currencies
            SaveListToCsvFileToPostgreSql("Currencies", schemaName, _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Save TimeZones
            SaveListToCsvFileToPostgreSql("TimeZones", schemaName, _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Save MetaZones
            SaveListToCsvFileToPostgreSql("MetaZones", schemaName, _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Save WindowsZones
            SaveListToCsvFileToPostgreSql("WindowsZones", schemaName, _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Save LanguageNames
            SaveListToCsvFileToPostgreSql("LanguageNames", schemaName, _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Save ScriptNames
            SaveListToCsvFileToPostgreSql("ScriptNames", schemaName, _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Save TerritoryNames
            SaveListToCsvFileToPostgreSql("TerritoryNames", schemaName, _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Save CurrencyNames
            SaveListToCsvFileToPostgreSql("CurrencyNames", schemaName, _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Save TimeZoneNames
            SaveListToCsvFileToPostgreSql("TimeZoneNames", schemaName, _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Save MetaZoneNames
            SaveListToCsvFileToPostgreSql("MetaZoneNames", schemaName, _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);

            RunPostgreSqlScript(connectionString, "PostgreSQL_Create_Schema_finalize.sql", "finalize Schema");
        }

        private void SaveListToCsvFileToPostgreSql<T>(string tableName, string schemaName, int rowCount, IList<T> list, string connectionString)
        {
            // PostgreSQL command: COPY table_name FROM file_name
            // COPY FROM can handle lines ending with newlines, carriage returns, or carriage return/newlines.
            // Have a look at: "https://www.postgresql.org/docs/current/static/sql-copy.html"

            // Create temp file and write to the destination.
            string fieldTerminator = ";";
            string quotingCharacter = "!";
            Console.WriteLine("Start copy {0} to PostgreSql DB.", tableName);

            try
            {
                var props = list.First().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                string columnNames = GetColumnNamesString(props);
                string tempCsvFile = CreateTempCsvFile(list, fieldTerminator, props, nullIdentifier: "\\N", useQuotingCharacter: quotingCharacter);
                // Needes a (empty) row before data! Else the first 'Code' will be inserted as corrupted.
                string content = File.ReadAllText(tempCsvFile);
                content = "\r\n" + content;
                File.WriteAllText(tempCsvFile, content);

                string scriptText = "SET search_path TO \"" + schemaName + "\"; COPY \"" + tableName + "\" " + columnNames + " FROM '" + tempCsvFile + "' WITH CSV HEADER DELIMITER '" + fieldTerminator + "' NULL '\\N' QUOTE '" + quotingCharacter + "' ENCODING 'UTF8';";
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    var insertData_cmd = new NpgsqlCommand(scriptText, conn);
                    conn.Open();
                    insertData_cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveListToCsvFileToPostgreSql on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }

            Console.WriteLine("\r{2}: Copied {0} rows from {1} to be done.", rowCount, rowCount, tableName);
        }

        private void CompareListToPostgreSql<T>(string tableName, string schemaName, int rowCount, IList<T> list, string connectionString)
        {
            string dbType = "PostgreSql-Table";
            int rowsInserted = 0;
            DataTable dataTable = new DataTable();

            Console.WriteLine("Start Compare {0}-List to {1}.", tableName, dbType);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT count(*) as rowsInserted FROM \"" + schemaName + "\".\"" + tableName + "\"", con))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                reader.Read();
                                rowsInserted = Convert.ToInt32(reader["rowsInserted"]);
                            }
                        }
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"" + schemaName + "\".\"" + tableName + "\"", con))
                    {
                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }

                Console.WriteLine("\r{2}: '{0}' rows of '{1}' inserted.", rowsInserted, rowCount, tableName);
                // if not equal, always compare each row!
                if (rowsInserted != rowCount)
                {
                    List<T> listInDB = ConvertDataTableToList<T>(dataTable);
                    CompareDBToListDetails(listInDB, list, tableName, rowCount, dbType);
                }
                else if (_ForceFullCompare)
                {
                    List<T> listInDB = ConvertDataTableToList<T>(dataTable);
                    CompareDBToListDetails(listInDB, list, tableName, rowCount, dbType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CompareListToPostgreSql on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}