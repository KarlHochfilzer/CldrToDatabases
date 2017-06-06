using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.IO;

namespace CldrToDatabases.DBModels
{
    /// <summary>
    /// Needed only for Oracle to get/set the OracleDbType used by 'OracleCommand.Parameters'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DBModelsOracleDbTypeAttribute : Attribute
    {
        public OracleDbType OracleDbTypeProp { get; private set; }

        /// <summary>
        /// Needed only for Oracle to get/set the OracleDbType used by 'OracleCommand.Parameters'.
        /// </summary>
        /// <param name="oracleDbTypeProp">OracleDbType to be set to.</param>
        public DBModelsOracleDbTypeAttribute(OracleDbType oracleDbTypeProp)
        {
            OracleDbTypeProp = oracleDbTypeProp;
        }
    }

    /// <summary>
    /// Needed only for MongoDB to get/set additional indexes, separated by IndexNumber fields ordered by FieldOrder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DBModelsMongoDbIndexAttribute : Attribute
    {
        public int IndexNumber { get; private set; }

        public int FieldOrder { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsUnique { get; private set; }

        /// <summary>
        /// Needed only for MongoDB to get/set indexes, separated by IndexNumber, fields ordered by FieldOrder.
        /// </summary>
        /// <param name="indexNumber">Identifies and index containing fields.</param>
        /// <param name="fieldOrder">Fields to be used in this order.</param>
        /// <param name="sortOrder">Field sort order ascending (1) or descending (-1).</param>
        /// <param name="isUnique">true if index is unique, default = false.</param>
        public DBModelsMongoDbIndexAttribute(int indexNumber, int fieldOrder, int sortOrder, bool isUnique = false)
        {
            IndexNumber = indexNumber;
            FieldOrder = fieldOrder;
            SortOrder = sortOrder;
            IsUnique = isUnique;
        }
    }

    public class DataDB : IDisposable
    {
        public DbContext dbContext = null;
        public DataDB(string _DBType, string projectPath, bool useDbContext)
        {
            if (useDbContext)
            {
                string pwd = GetPassword(projectPath);
                string connectionString = ConfigurationManager.ConnectionStrings[_DBType].ConnectionString.Replace("P_PPP_P", pwd);
                dbContext = new DbContext(_DBType);
                dbContext.Database.Connection.ConnectionString = connectionString;
            }
        }

        public void Dispose()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }

        public static DataDB Create(string _DBType, string projectPath, bool useDbContext)
        {
            return new DataDB(_DBType, projectPath, useDbContext);
        }

        private string GetPassword(string projectPath)
        {
            FileInfo file = new FileInfo(Path.Combine(projectPath, "pwd.txt"));
            string pwdtText = file.OpenText().ReadToEnd();

            return pwdtText;
        }
    }

    public class Languages
    {
        public Languages()
        {
        }

        public Languages(string code)
        {
            Code = code;
            IdStatus = string.Empty;
            ParentCode = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string IdStatus { get; set; }

        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ParentCode { get; set; }

        public override bool Equals(object obj)
        {
            Languages language = (Languages)obj;
            return (Code == language.Code && IdStatus == language.IdStatus && ParentCode == language.ParentCode);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ IdStatus.GetHashCode() ^ ParentCode.GetHashCode();
        }

        public override string ToString()
        {
            return "Languages: '" + Code + "', '" + IdStatus + "', '" + ParentCode + "'";
        }
    }

    public class Scripts
    {
        public Scripts()
        {
        }

        public Scripts(string code)
        {
            Code = code;
            IdStatus = string.Empty;
            LanguageCodes = string.Empty;
            LanguageAltCodes = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string IdStatus { get; set; }

        [DBModelsOracleDbType(OracleDbType.NClob)]
        public string LanguageCodes { get; set; }

        [DBModelsOracleDbType(OracleDbType.NClob)]
        public string LanguageAltCodes { get; set; }

        public override bool Equals(object obj)
        {
            Scripts script = (Scripts)obj;
            return (Code == script.Code && IdStatus == script.IdStatus && LanguageCodes == script.LanguageCodes && LanguageAltCodes == script.LanguageAltCodes);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ IdStatus.GetHashCode() ^ LanguageCodes.GetHashCode() ^ LanguageAltCodes.GetHashCode();
        }

        public override string ToString()
        {
            return "Scripts: '" + Code + "', '" + IdStatus + "', '" + LanguageCodes + "', '" + LanguageAltCodes + "'";
        }
    }

    public class Territories
    {
        public Territories()
        {
        }

        public Territories(string code)
        {
            Code = code;
            NumericCode = string.Empty;
            Alpha3Code = string.Empty;
            Fips10Code = string.Empty;
            // e.g. "" or "subdivision"
            Type = string.Empty;
            IdStatus = string.Empty;
            CurrencyCode = string.Empty;
            ParentCode = string.Empty;
            ParentGroupCodes = string.Empty;
            LanguageCodes = string.Empty;
            LanguageAltCodes = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string NumericCode { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Alpha3Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Fips10Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Type { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string IdStatus { get; set; }

        [DBModelsMongoDbIndex(3, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string CurrencyCode { get; set; }

        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ParentCode { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ParentGroupCodes { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string LanguageCodes { get; set; }

        [DBModelsOracleDbType(OracleDbType.NClob)]
        public string LanguageAltCodes { get; set; }

        public override bool Equals(object obj)
        {
            Territories territory = (Territories)obj;
            return (Code == territory.Code && NumericCode == territory.NumericCode && Alpha3Code == territory.Alpha3Code && Fips10Code == territory.Fips10Code && Type == territory.Type && IdStatus == territory.IdStatus && CurrencyCode == territory.CurrencyCode && ParentCode == territory.ParentCode && ParentGroupCodes == territory.ParentGroupCodes && LanguageCodes == territory.LanguageCodes && LanguageAltCodes == territory.LanguageAltCodes);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ NumericCode.GetHashCode() ^ Alpha3Code.GetHashCode() ^ Fips10Code.GetHashCode() ^ Type.GetHashCode() ^ IdStatus.GetHashCode() ^ CurrencyCode.GetHashCode() ^ ParentCode.GetHashCode() ^ ParentGroupCodes.GetHashCode() ^ LanguageCodes.GetHashCode() ^ LanguageAltCodes.GetHashCode();
        }

        public override string ToString()
        {
            return "Territories: '" + Code + "', '" + NumericCode + "', '" + Alpha3Code + "', '" + Fips10Code + "', '" + Type + "', '" + IdStatus + "', '" + CurrencyCode + "', '" + ParentCode + "', '" + ParentGroupCodes + "', '" + LanguageCodes + "', '" + LanguageAltCodes + "'";
        }
    }

    public class Currencies
    {
        public Currencies()
        {
        }

        public Currencies(string code)
        {
            Code = code;
            NumericCode = string.Empty;
            Description = string.Empty;
            IdStatus = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string NumericCode { get; set; }

        // Currencies.Description needed as fallback if at least no CurrencyNames.Locale 'en' entry exists
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Description { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string IdStatus { get; set; }

        public override bool Equals(object obj)
        {
            Currencies currency = (Currencies)obj;
            return (Code == currency.Code && NumericCode == currency.NumericCode && Description == currency.Description && IdStatus == currency.IdStatus);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ NumericCode.GetHashCode() ^ Description.GetHashCode() ^ IdStatus.GetHashCode();
        }

        public override string ToString()
        {
            return "Currencies: '" + Code + "', '" + NumericCode + "', '" + Description + "', '" + IdStatus + "'";
        }
    }

    public class TimeZones
    {
        public TimeZones()
        {
        }

        public TimeZones(string code)
        {
            Code = code;
            Description = string.Empty;
            MetaZoneCode = string.Empty;
            TerritoryCode = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        // TimeZones.Description needed as fallback if at least no LanguageNames.Locale 'en' entry exists
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Description { get; set; }

        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string MetaZoneCode { get; set; }

        [DBModelsMongoDbIndex(3, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string TerritoryCode { get; set; }

        // Standard-Time offset to UTC in seconds
        [DBModelsOracleDbType(OracleDbType.Decimal)]
        public int StandardOffset { get; set; }

        public override bool Equals(object obj)
        {
            TimeZones timeZone = (TimeZones)obj;
            return (Code == timeZone.Code && Description == timeZone.Description && MetaZoneCode == timeZone.MetaZoneCode && TerritoryCode == timeZone.TerritoryCode && StandardOffset == timeZone.StandardOffset);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Description.GetHashCode() ^ MetaZoneCode.GetHashCode() ^ TerritoryCode.GetHashCode() ^ StandardOffset.GetHashCode();
        }

        public override string ToString()
        {
            return "TimeZones: '" + Code + "', '" + Description + "', '" + MetaZoneCode + "', '" + TerritoryCode + "', '" + StandardOffset + "'";
        }
    }

    public class MetaZones
    {
        public MetaZones()
        {
        }

        public MetaZones(string code)
        {
            Code = code;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        public override bool Equals(object obj)
        {
            MetaZones metaZone = (MetaZones)obj;
            return (Code == metaZone.Code);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return "MetaZones: '" + Code + "'";
        }
    }

    public class WindowsZones
    {
        public WindowsZones()
        {
        }

        public WindowsZones(string code, string timeZoneCode, string territoryCode)
        {
            Code = code;
            TimeZoneCode = timeZoneCode;
            TerritoryCode = territoryCode;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string TimeZoneCode { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 3, 1, true)]
        [DBModelsMongoDbIndex(3, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string TerritoryCode { get; set; }

        public override bool Equals(object obj)
        {
            WindowsZones windowsZone = (WindowsZones)obj;
            return (Code == windowsZone.Code && TimeZoneCode == windowsZone.TimeZoneCode && TerritoryCode == windowsZone.TerritoryCode);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ TimeZoneCode.GetHashCode() ^ TerritoryCode.GetHashCode();
        }

        public override string ToString()
        {
            return "WindowsZones: '" + Code + "', '" + TimeZoneCode + "', '" + TerritoryCode + "'";
        }
    }

    public class LanguageNames
    {
        public LanguageNames()
        {
        }

        public LanguageNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            ShortName = string.Empty;
            LongName = string.Empty;
            VariantName = string.Empty;
            SecondaryName = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ShortName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string LongName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string VariantName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string SecondaryName { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            LanguageNames languageName = (LanguageNames)obj;
            return (Code == languageName.Code && Name == languageName.Name && ShortName == languageName.ShortName && LongName == languageName.LongName && VariantName == languageName.VariantName && SecondaryName == languageName.SecondaryName && Locale == languageName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ ShortName.GetHashCode() ^ LongName.GetHashCode() ^ VariantName.GetHashCode() ^ SecondaryName.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "LanguageNames: '" + Code + "', '" + Name + "', '" + ShortName + "', '" + LongName + "', '" + VariantName + "', '" + SecondaryName + "', '" + Locale + "'";
        }
    }

    public class ScriptNames
    {
        public ScriptNames()
        {
        }

        public ScriptNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            ShortName = string.Empty;
            StandAloneName = string.Empty;
            VariantName = string.Empty;
            SecondaryName = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ShortName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string VariantName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string StandAloneName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string SecondaryName { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            ScriptNames scriptName = (ScriptNames)obj;
            return (Code == scriptName.Code && Name == scriptName.Name && ShortName == scriptName.ShortName && VariantName == scriptName.VariantName && StandAloneName == scriptName.StandAloneName && SecondaryName == scriptName.SecondaryName && Locale == scriptName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ ShortName.GetHashCode() ^ VariantName.GetHashCode() ^ StandAloneName.GetHashCode() ^ SecondaryName.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "ScriptNames: '" + Code + "', '" + Name + "', '" + ShortName + "', '" + VariantName + "', '" + StandAloneName + "', '" + SecondaryName + "', '" + Locale + "'";
        }
    }

    public class TerritoryNames
    {
        public TerritoryNames()
        {
        }

        public TerritoryNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            ShortName = string.Empty;
            VariantName = string.Empty;
            // Set as "subdivision" on AddTerritorySubdivisionNameEntries
            Type = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ShortName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string VariantName { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Type { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            TerritoryNames territoryName = (TerritoryNames)obj;
            return (Code == territoryName.Code && Name == territoryName.Name && ShortName == territoryName.ShortName && VariantName == territoryName.VariantName && Type == territoryName.Type && Locale == territoryName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ ShortName.GetHashCode() ^ VariantName.GetHashCode() ^ Type.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "TerritoryNames: '" + Code + "', '" + Name + "', '" + ShortName + "', '" + VariantName + "', '" + Type + "', '" + Locale + "'";
        }
    }

    public class CurrencyNames
    {
        public CurrencyNames()
        {
        }

        public CurrencyNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            Symbol = string.Empty;
            SymbolNarrow = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Symbol { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string SymbolNarrow { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            CurrencyNames currencyName = (CurrencyNames)obj;
            return (Code == currencyName.Code && Name == currencyName.Name && Symbol == currencyName.Symbol && SymbolNarrow == currencyName.SymbolNarrow && Locale == currencyName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ Symbol.GetHashCode() ^ SymbolNarrow.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "CurrencyNames: '" + Code + "', '" + Name + "', '" + Symbol + "', '" + SymbolNarrow + "', '" + Locale + "'";
        }
    }

    public class TimeZoneNames
    {
        public TimeZoneNames()
        {
        }

        public TimeZoneNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            City = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string City { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            TimeZoneNames timeZoneName = (TimeZoneNames)obj;
            return (Code == timeZoneName.Code && Name == timeZoneName.Name && City == timeZoneName.City && Locale == timeZoneName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ City.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "TimeZoneNames: '" + Code + "', '" + Name + "', '" + City + "', '" + Locale + "'";
        }
    }

    public class MetaZoneNames
    {
        public MetaZoneNames()
        {
        }

        public MetaZoneNames(string code, string locale)
        {
            Code = code;
            Locale = locale;
            Name = string.Empty;
            ShortName = string.Empty;
        }

        [Key]
        [DBModelsMongoDbIndex(1, 2, 1, true)]
        [DBModelsMongoDbIndex(2, 1, 1)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Code { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Name { get; set; }

        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string ShortName { get; set; }

        [Key]
        [DBModelsMongoDbIndex(1, 1, 1, true)]
        [DBModelsOracleDbType(OracleDbType.NVarchar2)]
        public string Locale { get; set; }

        public override bool Equals(object obj)
        {
            MetaZoneNames metaZoneName = (MetaZoneNames)obj;
            return (Code == metaZoneName.Code && Name == metaZoneName.Name && ShortName == metaZoneName.ShortName && Locale == metaZoneName.Locale);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Name.GetHashCode() ^ ShortName.GetHashCode() ^ Locale.GetHashCode();
        }

        public override string ToString()
        {
            return "MetaZoneNames: '" + Code + "', '" + Name + "', '" + ShortName + "', '" + Locale + "'";
        }
    }
}
