using CldrToDatabases.DBModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CldrToDatabases.DataBuilder
{
    class DataToMySql : DataToDBs
    {
        public DataToMySql(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) : 
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: 5.7.9-log MySQL Community Server(GPL), Win64 (x86_64)

            // Connect only to Server, so remove the database from the existing connection string.
            // Else connection doesn't work if the database doesn't exist!
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString.Replace(";database=Countries", string.Empty);
            RunMySqlScript(connectionString, "MySQL_Create_DB.sql", "create");
        }

        public override void SaveDBData(DataDB dataDB)
        {
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == "MySql.Data.MySqlClient"

                // Attention!
                // You will not get an error on duplicate primary keys!
                // Additional keys will not be inserted! (No message!)

                string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
                SaveToMySql(connectionString);
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

            // Compare Languages
            CompareListToMySql("Languages", _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Compare Scripts
            CompareListToMySql("Scripts", _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Compare Territories
            CompareListToMySql("Territories", _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Compare Currencies
            CompareListToMySql("Currencies", _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Compare TimeZones
            CompareListToMySql("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Compare MetaZones
            CompareListToMySql("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Compare WindowsZones
            CompareListToMySql("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Compare LanguageNames
            CompareListToMySql("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Compare ScriptNames
            CompareListToMySql("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Compare TerritoryNames
            CompareListToMySql("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Compare CurrencyNames
            CompareListToMySql("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Compare TimeZoneNames
            CompareListToMySql("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Compare MetaZoneNames
            CompareListToMySql("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void RunMySqlScript(string connectionString, string scriptFileName, string todo)
        {
            Console.WriteLine("Start {0} MySql Database.", todo);
            try
            {
                FileInfo file = new FileInfo(Path.Combine(_ScriptsPath, scriptFileName));
                string scriptText = file.OpenText().ReadToEnd();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlScript script = new MySqlScript(conn, scriptText);
                    script.Delimiter = ";";
                    script.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-RunMySqlScript ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToMySql(string connectionString)
        {
            // Save Languages
            SaveListToCsvFileToMySql("Languages", _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Save Scripts
            SaveListToCsvFileToMySql("Scripts", _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Save Territories
            SaveListToCsvFileToMySql("Territories", _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Save Currencies
            SaveListToCsvFileToMySql("Currencies", _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Save TimeZones
            SaveListToCsvFileToMySql("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Save MetaZones
            SaveListToCsvFileToMySql("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Save WindowsZones
            SaveListToCsvFileToMySql("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Save LanguageNames
            SaveListToCsvFileToMySql("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Save ScriptNames
            SaveListToCsvFileToMySql("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Save TerritoryNames
            SaveListToCsvFileToMySql("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Save CurrencyNames
            SaveListToCsvFileToMySql("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Save TimeZoneNames
            SaveListToCsvFileToMySql("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Save MetaZoneNames
            SaveListToCsvFileToMySql("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);

            RunMySqlScript(connectionString, "MySQL_Create_DB_finalize.sql", "finalize");
        }

        private void SaveListToCsvFileToMySql<T>(string tableName, int rowCount, IList<T> list, string connectionString)
        {
            // Create temp file and write to the destination.
            string fieldTerminator = "\t";
            int rowsInserted = 0;
            Console.WriteLine("Start copy {0} to MySql DB.", tableName);

            try
            {
                var props = list.First().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                string tempCsvFile = CreateTempCsvFile(list, fieldTerminator, props, nullIdentifier: "\\N");
                // Needes a (empty) row before data! Else the first 'Code' will be inserted as corrupted.
                string content = File.ReadAllText(tempCsvFile);
                content = "\r\n" + content;
                File.WriteAllText(tempCsvFile, content);


                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlBulkLoader bulkLoader = new MySqlBulkLoader(conn);

                    bulkLoader.TableName = tableName;
                    bulkLoader.FileName = tempCsvFile;
                    bulkLoader.FieldTerminator = fieldTerminator;
                    bulkLoader.LineTerminator = "\r\n";
                    bulkLoader.NumberOfLinesToSkip = 1;
                    rowsInserted = bulkLoader.Load();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveListToCsvFileToMySql on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }

            Console.WriteLine("\r{2}: Copied {0} rows from {1} to be done.", rowsInserted, rowCount, tableName);
        }

        private void CompareListToMySql<T>(string tableName, int rowCount, IList<T> list, string connectionString)
        {
            string dbType = "MySql-Table";
            int rowsInserted = 0;
            DataTable dataTable = new DataTable();

            Console.WriteLine("Start Compare {0}-List to {1}.", tableName, dbType);

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT count(*) as rowsInserted FROM " + tableName, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                reader.Read();
                                rowsInserted = Convert.ToInt32(reader["rowsInserted"]);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + tableName, con))
                    {
                        using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd))
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
                Console.WriteLine("*** ERROR-CompareListToMySql on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}
