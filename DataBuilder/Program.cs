using CldrToDatabases.DataBuilder;
using System;
using System.IO;

namespace CldrToDatabases
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //string directory = Directory.GetCurrentDirectory();
            //string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase);
            //string directoryName = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            // Best to use:
            string projectPath = AppDomain.CurrentDomain.BaseDirectory;
            // Attention! Replace is case sensitive!
            projectPath = projectPath.Replace(@"\Release", string.Empty).Replace(@"\Debug", string.Empty).Replace(@"\bin", string.Empty);
            var dataPath = Path.Combine(projectPath, "data");
            DataExtractor extractor = DataExtractor.Load(dataPath, downloadData: false);

            // Currently available DBType:
            // "MsSqlConnection", "MySqlConnection", "PostgreSqlConnection", "SQLiteConnection", "OracleConnection", "MongoDBConnection"
            // Setup DBType in App.config, attribute 'name', within "<connectionStrings>" as <add name="YOUR_DBType" ... />
            IDataToDBs saveDataToBBs = DataToDBs.SaveDataToDBs(projectPath, dataPath, DBType: "OracleConnection", createDatabase: true, dataExtractor: extractor, forceFullCompare: true);
            saveDataToBBs.Save();

            Console.WriteLine();
            Console.WriteLine("Imports done.\r\nPress Enter to finish.");
            Console.Read();
        }
    }
}
