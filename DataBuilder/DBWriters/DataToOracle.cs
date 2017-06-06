using CldrToDatabases.DBModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

namespace CldrToDatabases.DataBuilder
{
    class DataToOracle : DataToDBs
    {
        public DataToOracle(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) : 
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: Oracle Database Express Edition 11g Release 2 for Windows x64
            // Read installation instructions BEFORE install ODAC!!!
            // ODAC 12c Release 4 and Oracle Developer Tools for Visual Studio (12.1.0.2.4)
            // Doesn't create a Database!
            // Creates a User ("Countries") without any privilege settings and his schema objects.

            // Connect to Server as 'SYS AS SYSDBA'.
            // Else script doesn't work!
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
            RunOracleScript(connectionString, "Oracle_Create_Schema.sql", ";", "create User-Schema");
        }

        public override void SaveDBData(DataDB dataDB)
        {
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == "Oracle.ManagedDataAccess.Client"

                string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
                SaveToOracle(connectionString);
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
            string userName = "Countries";

            // Compare Languages
            CompareListToOracle("Languages", userName, _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Compare Scripts
            CompareListToOracle("Scripts", userName, _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Compare Territories
            CompareListToOracle("Territories", userName, _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Compare Currencies
            CompareListToOracle("Currencies", userName, _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Compare TimeZones
            CompareListToOracle("TimeZones", userName, _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Compare MetaZones
            CompareListToOracle("MetaZones", userName, _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Compare WindowsZones
            CompareListToOracle("WindowsZones", userName, _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Compare LanguageNames
            CompareListToOracle("LanguageNames", userName, _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Compare ScriptNames
            CompareListToOracle("ScriptNames", userName, _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Compare TerritoryNames
            CompareListToOracle("TerritoryNames", userName, _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Compare CurrencyNames
            CompareListToOracle("CurrencyNames", userName, _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Compare TimeZoneNames
            CompareListToOracle("TimeZoneNames", userName, _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Compare MetaZoneNames
            CompareListToOracle("MetaZoneNames", userName, _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void RunOracleScript(string connectionString, string fileName, string commandSeparator, string todo)
        {
            Console.WriteLine("Start {0} Oracle Database.", todo);

            string currentCommand = "";
            try
            {
                FileInfo scriptFile = new FileInfo(Path.Combine(_ScriptsPath, fileName));
                string scriptText = scriptFile.OpenText().ReadToEnd();
                string[] commands = scriptText.Split(new[] { commandSeparator }, StringSplitOptions.RemoveEmptyEntries);
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleCommand command = new OracleCommand();
                    command.Connection = conn;
                    command.CommandType = CommandType.Text;
                    foreach (var cmd in commands)
                    {
                        if (string.IsNullOrWhiteSpace(cmd.Replace("\r\n", " ")))
                            continue;
                        if (cmd.Contains("ALTER SESSION"))
                            currentCommand = cmd.Replace(";", "");
                        else
                            currentCommand = cmd;
                        command.CommandText = currentCommand;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-RunOracleScript ***:\r\nMessage: {0}\r\nStackTrace: {1}\r\nCommand: {2}", ex.Message, ex.StackTrace, currentCommand);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToOracle(string connectionString)
        {
            string userName = "Countries";

            // Save Languages
            SaveListToOracle("Languages", userName, _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Save Scripts
            SaveListToOracle("Scripts", userName, _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Save Territories
            SaveListToOracle("Territories", userName, _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Save Currencies
            SaveListToOracle("Currencies", userName, _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Save TimeZones
            SaveListToOracle("TimeZones", userName, _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Save MetaZones
            SaveListToOracle("MetaZones", userName, _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Save WindowsZones
            SaveListToOracle("WindowsZones", userName, _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Save LanguageNames
            SaveListToOracle("LanguageNames", userName, _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Save ScriptNames
            SaveListToOracle("ScriptNames", userName, _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Save TerritoryNames
            SaveListToOracle("TerritoryNames", userName, _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Save CurrencyNames
            SaveListToOracle("CurrencyNames", userName, _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Save TimeZoneNames
            SaveListToOracle("TimeZoneNames", userName, _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Save MetaZoneNames
            SaveListToOracle("MetaZoneNames", userName, _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);

            // create constraints
            RunOracleScript(connectionString, "Oracle_Create_Schema_finalize.sql", ";", "finalize");
        }

        private void SaveListToOracle<T>(string tableName, string userName, int rowCount, IList<T> list, string connectionString)
        {
            // You also can use "SQL*Loader (sqlldr)" to insert large amount of data from (csv)file.
            // Have a look at: "http://www.orafaq.com/wiki/SQL*Loader_FAQ"
            int rowsInserted = 0;
            Console.WriteLine("Start copy {0} to Oracle DB.", tableName);

            try
            {

                PropertyInfo[] props = list.First().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<string> strList = new List<string>();
                Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
                List<OracleDbType> oraColumnTypes = new List<OracleDbType>();

                string columnNames = GetColumnNamesString(props);
                string valueNames = GetValueNamesString(props);
                foreach (var prop in props)
                {
                    DBModelsOracleDbTypeAttribute attribute = (DBModelsOracleDbTypeAttribute)prop.GetCustomAttribute(typeof(DBModelsOracleDbTypeAttribute));
                    oraColumnTypes.Add(attribute.OracleDbTypeProp);
                    dataDict.Add(":" + prop.Name, new List<string>());
                }
                // Store each column and their values in a dictionary containing the column-name and a list of all values
                foreach (var row in list)
                {
                    foreach (var p in props)
                    {
                        string val = p.GetValue(row, null)?.ToString();
                        dataDict[":" + p.Name].Add(val);
                    }
                }
                string insertStatement = "INSERT INTO \"" + userName + "\".\"" + tableName + "\" " + columnNames + " VALUES " + valueNames;
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = insertStatement;
                        command.CommandType = CommandType.Text;
                        command.BindByName = true;
                        // In order to use ArrayBinding: the ArrayBindCount property
                        // of OracleCommand object must be set to the number of records to be inserted
                        command.ArrayBindCount = rowCount;
                        OracleDbType[] oraType = oraColumnTypes.ToArray();
                        int i = 0;
                        // Loop each column-name (data.Key)
                        foreach (var data in dataDict)
                        {
                            command.Parameters.Add(data.Key, oraType[i], data.Value.ToArray(), ParameterDirection.Input);
                            i++;
                        }
                        rowsInserted = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveListToDataTableToOracle on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }

            Console.WriteLine("\r{2}: Copied {0} rows from {1} to be done.", rowsInserted, rowCount, tableName);
        }

        private void CompareListToOracle<T>(string tableName, string userName, int rowCount, IList<T> list, string connectionString)
        {
            string dbType = "Oracle-Table";
            int rowsInserted = 0;
            DataTable dataTable = new DataTable();

            Console.WriteLine("Start Compare {0}-List to {1}.", tableName, dbType);

            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("SELECT count(*) as rowsInserted FROM \"" + userName + "\".\"" + tableName + "\"", con))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                reader.Read();
                                rowsInserted = Convert.ToInt32(reader["rowsInserted"]);
                            }
                        }
                    }
                    using (OracleCommand cmd = new OracleCommand("SELECT * FROM \"" + userName + "\".\"" + tableName + "\"", con))
                    {
                        using (OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd))
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
                Console.WriteLine("*** ERROR-CompareListToOracle on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}
