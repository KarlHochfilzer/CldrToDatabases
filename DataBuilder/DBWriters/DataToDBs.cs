using CldrToDatabases.DBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CldrToDatabases.DataBuilder
{
    public class DataToDBs : IDataToDBs
    {
        protected readonly string _ProjectPath;
        protected readonly string _ScriptsPath;
        protected readonly string _DataPath;
        protected readonly string _DBType;
        protected readonly bool _CreateDatabase;
        protected readonly DataExtractor _DataExtractor;
        protected readonly bool _ForceFullCompare;

        protected int _RowsToInsert;
        protected string _TableToInsert;
        protected readonly int _LngCount;
        protected readonly int _ScrCount;
        protected readonly int _TerCount;
        protected readonly int _CurCount;
        protected readonly int _TzoCount;
        protected readonly int _MtzCount;
        protected readonly int _WinCount;
        protected readonly int _LngNaCount;
        protected readonly int _ScrNaCount;
        protected readonly int _TerNaCount;
        protected readonly int _CurNaCount;
        protected readonly int _TzoNaCount;
        protected readonly int _MtzNaCount;

        public static IDataToDBs DataToDBsInterface;

        public DataToDBs(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare)
        {
            _ProjectPath = projectPath;
            _ScriptsPath = Path.Combine(projectPath, "DBScripts");
            _DataPath = dataPath + "\\";
            _DBType = DBType;
            _CreateDatabase = createDatabase;
            _DataExtractor = dataExtractor;
            _ForceFullCompare = forceFullCompare;

            // Set data counts used for output
            _LngCount = dataExtractor.LanguagesList.Count;
            _ScrCount = dataExtractor.ScriptsList.Count;
            _TerCount = dataExtractor.TerritoriesList.Count;
            _CurCount = dataExtractor.CurrenciesList.Count;
            _TzoCount = dataExtractor.TimeZonesList.Count;
            _MtzCount = dataExtractor.MetaZonesList.Count;
            _WinCount = dataExtractor.WindowsZonesList.Count;

            _LngNaCount = dataExtractor.LanguageNamesList.Count;
            _ScrNaCount = dataExtractor.ScriptNamesList.Count;
            _TerNaCount = dataExtractor.TerritoryNamesList.Count;
            _CurNaCount = dataExtractor.CurrencyNamesList.Count;
            _TzoNaCount = dataExtractor.TimeZoneNamesList.Count;
            _MtzNaCount = dataExtractor.MetaZoneNamesList.Count;
        }

        public static IDataToDBs SaveDataToDBs(string projectPath, string dataPath, string DBType, bool createDatabase, DataExtractor dataExtractor, bool forceFullCompare = false)
        {
            // eg. change from connectionStrings name "MsSqlConnection" to Classname "DataToMsSql"
            string className = "DataTo" + DBType.Replace("Connection", string.Empty);
            // All derived classes have to be in the same namepsace!
            string namespaceName = typeof(DataToDBs).Namespace;
            try
            {
                DataToDBsInterface = (IDataToDBs)Activator.CreateInstance(Type.GetType(namespaceName + "." + className), projectPath, dataPath, DBType, createDatabase, dataExtractor, forceFullCompare);
            }
            catch
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-SaveDataToDBs ***:\r\nMessage: Class: '{0}' not defined!", className);
                Console.WriteLine("*** ERROR-SaveDataToDBs ***:\r\nDatabase-Client '{0}' not provided!", DBType);
                Console.Read();
                Environment.Exit(-1);
            }

            return DataToDBsInterface;
        }

        // Currently function has only one override in 'DataToMongoDB' class.
        public virtual bool UseDbContext()
        {
            return true;
        }

        public void Save()
        {
            try
            {
                using (var dataDB = DataDB.Create(_DBType, _ProjectPath, DataToDBsInterface.UseDbContext()))
                {
                    if (_CreateDatabase)
                    {
                        DataToDBsInterface.CreateDB(dataDB);
                    }
                    DataToDBsInterface.SaveDBData(dataDB);

                    DataToDBsInterface.CompareDBDataToLists(dataDB);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-Save ***:\r\nMessage: Function 'save()' for: '{0}' not defined!\r\nException: {1}", _DBType, ex.Message);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        public virtual void CreateDB(DataDB dataDB)
        {

            Console.WriteLine();
            Console.WriteLine("*** ERROR-CreateDB ***:\r\nMessage: {0}", "Function 'CreateDB(..)' for : \"" + _DBType + "\" not defined!");
            Console.Read();
            Environment.Exit(-1);
        }

        public virtual void SaveDBData(DataDB dataDB)
        {
            Console.WriteLine();
            Console.WriteLine("*** ERROR-SaveDBData ***:\r\nMessage: {0}", "Function 'SaveDBData(..)' for : \"" + _DBType + "\" not defined!");
            Console.Read();
            Environment.Exit(-1);
        }

        public virtual void CompareDBDataToLists(DataDB dataDB)
        {
            Console.WriteLine();
            Console.WriteLine("*** ERROR-CompareDBDataToLists ***:\r\nMessage: {0}", "Function 'CompareDBDataToLists(..)' for : \"" + _DBType + "\" not defined!");
            Console.Read();
            Environment.Exit(-1);
        }

        protected void CompareDBToListDetails<T>(IList<T> listInDB, IList<T> list, string tableName, int rowCount, string dbType)
        {
            int currentRow = 0;
            int notifyRow = 0;
            int notifyEach = 100;
            Console.WriteLine("Start CompareDetails {0}-List to {1}.", tableName, dbType);

            try
            {
                List<T> compareList = list.ToList();
                List<T> notEqualsInDB = new List<T>();
                List<T> notEqualsInList = new List<T>();
                foreach (var item in listInDB)
                {
                    if (!compareList.Remove(item))
                    {
                        notEqualsInDB.Add(item);
                        notEqualsInList.Add(GetFirstElementDynamic(list, item, tableName));
                    }

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

                if (compareList.Any())
                {
                    int countNotEqual = notEqualsInDB.Count();
                    Console.WriteLine("CompareDetails on {0}, {1} Elements not equal in DB!", tableName, countNotEqual.ToString());
                    foreach (var notEqualDB in notEqualsInDB)
                    {
                        Console.WriteLine("Not equal data in DB: \"{0}\"", notEqualDB.ToString());
                    }
                    foreach (var notEqualLi in notEqualsInList)
                    {
                        Console.WriteLine("Not equal data should be: \"{0}\"", notEqualLi.ToString());
                        compareList.Remove(notEqualLi);
                    }
                    int countMissing = compareList.Count();
                    Console.WriteLine("CompareDetails on {0}, {1} Elements missing!", tableName, countMissing.ToString());
                    foreach (var missing in compareList)
                    {
                        Console.WriteLine("Missing: \"{0}\"", missing.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR-CompareDetails on {0} ***:\r\nMessage: {1}\r\nStackTrace: {2}", tableName, ex.Message, ex.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
        }

        protected DataTable ConvertListToDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor propD in properties)
                table.Columns.Add(propD.Name, Nullable.GetUnderlyingType(propD.PropertyType) ?? propD.PropertyType);
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor propD in properties)
                    row[propD.Name] = propD.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        protected List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = Activator.CreateInstance<T>();
                foreach (PropertyDescriptor propD in properties)
                {
                    object value;
                    if (row[propD.Name].GetType() == typeof(byte[]))
                    {
                        value = Encoding.UTF8.GetString((byte[])row[propD.Name]);
                    }
                    else if (row[propD.Name].GetType() == typeof(decimal))
                    {
                        value = decimal.ToInt32((decimal)row[propD.Name]);
                    }
                    else
                    {
                        value = row[propD.Name] == DBNull.Value ? "" : row[propD.Name];
                    }
                    propD.SetValue(item, value);
                }
                list.Add(item);
            }
            return list;
        }

        protected string CreateTempCsvFile<T>(IList<T> list, string fieldTerminator, PropertyInfo[] props, string nullIdentifier = "", string useQuotingCharacter = "")
        {
            string tempCsvFile = Path.Combine(_DataPath, "temp.csv");
            File.Delete(tempCsvFile);

            List<string> strList = new List<string>();
            foreach (var row in list)
            {
                var rowProps = props.Select(p => p.GetValue(row, null)?.ToString());
                string itemStr;
                if (useQuotingCharacter != "")
                {
                    itemStr = string.Join(fieldTerminator, rowProps.Select(rp => rp == "" ? rp = nullIdentifier : useQuotingCharacter + rp + useQuotingCharacter).ToArray());
                }
                else
                {
                    itemStr = string.Join(fieldTerminator, rowProps.Select(rp => rp == "" ? rp = nullIdentifier : rp).ToArray());
                }
                strList.Add(itemStr);
            }
            File.WriteAllLines(tempCsvFile, strList, Encoding.UTF8);
            return tempCsvFile;
        }

        protected string GetColumnNamesString(PropertyInfo[] props)
        {
            string columnNames = "(" + string.Join(", ", props.Select(p => "\"" + p.Name + "\"").ToArray()) + ")";
            return columnNames;
        }

        protected string GetValueNamesString(PropertyInfo[] props)
        {
            string valueNames = "(" + string.Join(", ", props.Select(p => ":" + p.Name).ToArray()) + ")";
            return valueNames;
        }

        protected List<string> GetKeyColumns<T>(T obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> indexColumns = new List<string>();
            foreach (var prop in props)
            {
                KeyAttribute attributes = (KeyAttribute)prop.GetCustomAttribute(typeof(KeyAttribute));
                if (attributes != null)
                {
                    indexColumns.Add(prop.Name);
                }
            }

            return indexColumns;
        }

        protected T GetFirstElementDynamic<T>(IList<T> list, T element, string tableName)
        {
            // Using Expression Trees
            List<Expression> exList = new List<Expression>();
            List<string> keyColumns = GetKeyColumns(element);

            IQueryable<T> queryableData = list.AsQueryable<T>();
            ParameterExpression pe = Expression.Parameter(typeof(T), tableName);
            Expression left;
            Expression right;

            foreach (var keyColumn in keyColumns)
            {
                left = Expression.Property(pe, typeof(T).GetProperty(keyColumn));
                right = Expression.Constant(typeof(T).GetProperty(keyColumn).GetValue(element).ToString());
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

            return results.First();
        }
    }
}