using CldrToDatabases.DBModels;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CldrToDatabases.DataBuilder
{
    public class DataToMsSql : DataToDBs
    {
        public DataToMsSql(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare) : 
            base(projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare)
        {
        }

        public override void CreateDB(DataDB dataDB)
        {
            // Local installed:
            // Version: Microsoft SQL Server Express (64-bit), 13.0.4202.2 (= 2016 Express)

            // If SqlServer 2016 is installed and in use, you have to use Version '13.0.0.0', not '13.100.0.0' on:
            // References 'Microsoft.SqlServer.ConnectionInfo' and 'Microsoft.SqlServer.Smo'
            // Else you get an ERROR!!
            // "Could not load file or assembly 'microsoft.sqlserver.sqlclrprovider version=13.100.0.0'"

            // Connect only to Server, so remove the initial catalog from the existing connection string.
            // Else connection doesn't work if the Database doesn't exist!
            string connectionString = dataDB.dbContext.Database.Connection.ConnectionString.Replace(";initial catalog=Countries", string.Empty);
            RunMsSqlScript(connectionString, "MSSQL_Create_DB.sql", "create");
        }

        public override void SaveDBData(DataDB dataDB)
        {
            try
            {
                Console.CursorVisible = false;

                //var DBType = dataDB.dbContext.Database.Connection.GetType();
                //DBType.Namespace == "System.Data.SqlClient"

                // Attention!
                // You will not get an error on duplicate primary keys!
                // Additional keys will not be inserted! (No message!)

                string connectionString = dataDB.dbContext.Database.Connection.ConnectionString;
                SaveToMsSql(connectionString);
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
            CompareListToMsSql("dbo.Languages", _LngCount, _DataExtractor.LanguagesList, connectionString);
            // Compare Scripts
            CompareListToMsSql("dbo.Scripts", _ScrCount, _DataExtractor.ScriptsList, connectionString);
            // Compare Territories
            CompareListToMsSql("dbo.Territories", _TerCount, _DataExtractor.TerritoriesList, connectionString);
            // Compare Currencies
            CompareListToMsSql("dbo.Currencies", _CurCount, _DataExtractor.CurrenciesList, connectionString);
            // Compare TimeZones
            CompareListToMsSql("dbo.TimeZones", _TzoCount, _DataExtractor.TimeZonesList, connectionString);
            // Compare MetaZones
            CompareListToMsSql("dbo.MetaZones", _MtzCount, _DataExtractor.MetaZonesList, connectionString);
            // Compare WindowsZones
            CompareListToMsSql("dbo.WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, connectionString);

            // Locale Data

            // Compare LanguageNames
            CompareListToMsSql("dbo.LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, connectionString);
            // Compare ScriptNames
            CompareListToMsSql("dbo.ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, connectionString);
            // Compare TerritoryNames
            CompareListToMsSql("dbo.TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, connectionString);
            // Compare CurrencyNames
            CompareListToMsSql("dbo.CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, connectionString);
            // Compare TimeZoneNames
            CompareListToMsSql("dbo.TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, connectionString);
            // Compare MetaZoneNames
            CompareListToMsSql("dbo.MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, connectionString);
        }

        private void RunMsSqlScript(string connectionString, string scriptFileName, string todo)
        {
            Console.WriteLine("Start {0} MS-Sql Database.", todo);
            try
            {
                FileInfo file = new FileInfo(Path.Combine(_ScriptsPath, scriptFileName));
                string scriptText = file.OpenText().ReadToEnd();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    Server server = new Server(new ServerConnection(conn));
                    server.ConnectionContext.ExecuteNonQuery(scriptText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-RunMsSqlScript ***:\r\nMessage: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void SaveToMsSql(string connectionString)
        {
            // Save Languages
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.Languages", _LngCount, _DataExtractor.LanguagesList, bulkCopy);
            }
            // Save Scripts
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.Scripts", _ScrCount, _DataExtractor.ScriptsList, bulkCopy);
            }
            // Save Territories
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.Territories", _TerCount, _DataExtractor.TerritoriesList, bulkCopy);
            }
            // Save Currencies
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.Currencies", _CurCount, _DataExtractor.CurrenciesList, bulkCopy);
            }
            // Save TimeZones
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.TimeZones", _TzoCount, _DataExtractor.TimeZonesList, bulkCopy);
            }
            // Save MetaZones
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.MetaZones", _MtzCount, _DataExtractor.MetaZonesList, bulkCopy);
            }
            // Save WindowsZones
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.WindowsZones", _WinCount, _DataExtractor.WindowsZonesList, bulkCopy);
            }

            // Locale Data

            // Save LanguageNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.LanguageNames", _LngNaCount, _DataExtractor.LanguageNamesList, bulkCopy);
            }
            // Save ScriptNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.ScriptNames", _ScrNaCount, _DataExtractor.ScriptNamesList, bulkCopy);
            }
            // Save TerritoryNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.TerritoryNames", _TerNaCount, _DataExtractor.TerritoryNamesList, bulkCopy);
            }
            // Save CurrencyNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.CurrencyNames", _CurNaCount, _DataExtractor.CurrencyNamesList, bulkCopy);
            }
            // Save TimeZoneNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.TimeZoneNames", _TzoNaCount, _DataExtractor.TimeZoneNamesList, bulkCopy);
            }
            // Save MetaZoneNames
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                BulkCopyList("dbo.MetaZoneNames", _MtzNaCount, _DataExtractor.MetaZoneNamesList, bulkCopy);
            }

            RunMsSqlScript(connectionString, "MSSQL_Create_DB_finalize.sql", "finalize");
        }

        private void BulkCopyList<T>(string tableName, int rowCount, IList<T> list, SqlBulkCopy bulkCopy)
        {
            _TableToInsert = tableName.Replace("dbo.", string.Empty);
            _RowsToInsert = rowCount;

            bulkCopy.DestinationTableName = tableName;
            // Convert DBModels List to DataTable.
            DataTable table = ConvertListToDataTable(list);

            // Set up the event handler to notify each 100 rows.
            bulkCopy.NotifyAfter = 100;
            bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

            try
            {
                // Write from the DataTable to the destination.
                Console.WriteLine("Start copy {0} to MsSql DB.", _TableToInsert);
                bulkCopy.WriteToServer(table);
                Console.Write("\r{2}: Copied {0} rows from {1} to be done.", _RowsToInsert, _RowsToInsert, _TableToInsert);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-BulkCopyList on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", _TableToInsert, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write("\r{2}: Copied {0} rows from {1} to be done.", e.RowsCopied, _RowsToInsert, _TableToInsert);
        }

        private void CompareListToMsSql<T>(string tableName, int rowCount, IList<T> list, string connectionString)
        {
            string tableNameToCompare = tableName.Replace("dbo.", string.Empty);
            string dbType = "MsSql-Table";
            int rowsInserted = 0;
            DataTable dataTable = new DataTable();

            Console.WriteLine("Start Compare {0}-List to {1}.", tableNameToCompare, dbType);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT count(*) as rowsInserted FROM " + tableName, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                reader.Read();
                                rowsInserted = Convert.ToInt32(reader["rowsInserted"]);
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM " + tableName, con))
                    {
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }

                Console.WriteLine("\r{2}: '{0}' rows of '{1}' inserted.", rowsInserted, rowCount, tableNameToCompare);
                // if not equal, always compare each row!
                if (rowsInserted != rowCount)
                {
                    List<T> listInDB = ConvertDataTableToList<T>(dataTable);
                    CompareDBToListDetails(listInDB, list, tableNameToCompare, rowCount, dbType);
                }
                else if (_ForceFullCompare)
                {
                    List<T> listInDB = ConvertDataTableToList<T>(dataTable);
                    CompareDBToListDetails(listInDB, list, tableNameToCompare, rowCount, dbType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CompareListToMsSql on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableNameToCompare, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }
    }
}
