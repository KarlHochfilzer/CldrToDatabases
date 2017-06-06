using CldrToDatabases.DBModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace CldrToDatabases.DataBuilder
{
    class DataToSQLite : DataToDBs
    {
        private string _DBFile;
        private string _SQLiteDir;

        public DataToSQLite(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) : 
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
            _SQLiteDir = Path.Combine(_ProjectPath, "SQLite");
            _DBFile = Path.Combine(_SQLiteDir, "Countries.db");
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: SQLite version 3.7.3
            // Using: Sqliteman 1.2.2 and DB.Browser.for.SQLite-3.9.1-win64
            // Removed Password from connection string because both can not open a DB with Password.

            Console.WriteLine("Start create new SQLite DB-File.");
            if (!Directory.Exists(_SQLiteDir))
                Directory.CreateDirectory(_SQLiteDir);
            SQLiteConnection.CreateFile(_DBFile); // creates a zero-byte file (overwrites an existing file)

            // Connect to Data Source=dbFile.
            // Password is not set in connection string (eg. Password=P_PPP_P) because used tools can not open a DB with Password.
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString.Replace("Data Source=Countries.db;", "Data Source=" + _DBFile + ";");
            CreateSQLiteTables(connectionString);
        }

        public override void SaveDBData(DataDB dataDB)
        {
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == "System.Data.SQLite"

                // Connect to Data Source=dbFile.
                // Password is not set in connection string (eg. Password=P_PPP_P) because used tools can not open a DB with Password.
                string connectionString = dataDB.dbContext.Database.Connection.ConnectionString.Replace("Data Source=Countries.db;", "Data Source=" + _DBFile + ";");
                SaveToSQLite(connectionString);
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
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString.Replace("Data Source=Countries.db;", "Data Source=" + _DBFile + ";");

            // Compare Languages
            CompareListToSQLite("Languages", _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Compare Scripts
            CompareListToSQLite("Scripts", _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Compare Territories
            CompareListToSQLite("Territories", _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Compare Currencies
            CompareListToSQLite("Currencies", _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Compare TimeZones
            CompareListToSQLite("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Compare MetaZones
            CompareListToSQLite("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Compare WindowsZones
            CompareListToSQLite("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Compare LanguageNames
            CompareListToSQLite("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Compare ScriptNames
            CompareListToSQLite("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Compare TerritoryNames
            CompareListToSQLite("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Compare CurrencyNames
            CompareListToSQLite("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Compare TimeZoneNames
            CompareListToSQLite("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Compare MetaZoneNames
            CompareListToSQLite("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void CreateSQLiteTables(string connectionString)
        {
            Console.WriteLine("Start create SQLite Tables.");
            try
            {
                FileInfo file = new FileInfo(Path.Combine(_ScriptsPath, "SQLite_Create_DB.sql"));
                string scriptText = file.OpenText().ReadToEnd();
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand(scriptText, conn);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CreateSQLiteTables ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToSQLite(string connectionString)
        {
            // Save Languages
            SaveListToDataTableToSQLite("Languages", _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Save Scripts
            SaveListToDataTableToSQLite("Scripts", _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Save Territories
            SaveListToDataTableToSQLite("Territories", _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Save Currencies
            SaveListToDataTableToSQLite("Currencies", _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Save TimeZones
            SaveListToDataTableToSQLite("TimeZones", _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Save MetaZones
            SaveListToDataTableToSQLite("MetaZones", _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Save WindowsZones
            SaveListToDataTableToSQLite("WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Save LanguageNames
            SaveListToDataTableToSQLite("LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Save ScriptNames
            SaveListToDataTableToSQLite("ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Save TerritoryNames
            SaveListToDataTableToSQLite("TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Save CurrencyNames
            SaveListToDataTableToSQLite("CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Save TimeZoneNames
            SaveListToDataTableToSQLite("TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Save MetaZoneNames
            SaveListToDataTableToSQLite("MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void SaveListToDataTableToSQLite<T>(string tableName, int rowCount, IList<T> list, string connectionString)
        {
            int rowsInserted = 0;
            Console.WriteLine("Start copy {0} to SQLite DB.", tableName);

            try
            {
                // Convert DBModels List to DataTable.
                DataTable table = ConvertListToDataTable(list);

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    // Has to be stored within a 'SQLiteTransaction'.
                    // Else each row on update 'SQLiteDataAdapter.Update' has its own transaction. That's extremly slow!!
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        var sqliteAdapter = new SQLiteDataAdapter("SELECT * FROM '" + tableName + "'", conn);
                        var cmdBuilder = new SQLiteCommandBuilder(sqliteAdapter);
                        rowsInserted = sqliteAdapter.Update(table);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveListToDataTableToSQLite on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }

            Console.WriteLine("\r{2}: Copied {0} rows from {1} to be done.", rowsInserted, rowCount, tableName);
        }

        private void CompareListToSQLite<T>(string tableName, int rowCount, IList<T> list, string connectionString)
        {
            string dbType = "SQLite-Table";
            int rowsInserted = 0;
            DataTable dataTable = new DataTable();

            Console.WriteLine("Start Compare {0}-List to {1}.", tableName, dbType);

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT count(*) as rowsInserted FROM " + tableName, con))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                reader.Read();
                                rowsInserted = Convert.ToInt32(reader["rowsInserted"]);
                            }
                        }
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + tableName, con))
                    {
                        using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd))
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
                Console.WriteLine("*** ERROR-CompareListToSQLite on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}
