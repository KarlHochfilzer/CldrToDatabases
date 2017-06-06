-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : Script to create User "Countries" and his Schema objects without Triggers.

-- All Schema objects are created case sensitive!

-- Using Express Edition 11.2 defaults.
-- =============================================
-- A User that is currently connected to a database cannot be dropped, you must first terminate the User sessions.
-- User "Countries" without any privilege settings!

-- 1) Drop and create User.

DROP USER "Countries" CASCADE;

COMMIT;

CREATE USER "Countries"
    IDENTIFIED BY MyPassword
    DEFAULT TABLESPACE "USERS"
    QUOTA UNLIMITED ON USERS
    TEMPORARY TABLESPACE "TEMP";

COMMIT;

--2) Create User's Schema objects.

-- No need to always prepend the schema(user)name!
ALTER SESSION SET CURRENT_SCHEMA = "Countries";

/****** Object:  Table [Languages] ******/

CREATE TABLE "Languages"(
	"Code" nvarchar2(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	"IdStatus" nvarchar2(20) NULL,
	"ParentCode" nvarchar2(10) NULL,
        CONSTRAINT "PK_Languages" PRIMARY KEY ("Code")
);


/****** Object:  Table [Scripts] ******/

CREATE TABLE "Scripts"(
	"Code" nvarchar2(4) NOT NULL,
	"IdStatus" nvarchar2(20) NULL,
	"LanguageCodes" nclob NULL,
	"LanguageAltCodes" nclob NULL,
        CONSTRAINT "PK_Scripts" PRIMARY KEY ("Code")
);


/****** Object:  Table [Territories] ******/

CREATE TABLE "Territories"(
	"Code" nvarchar2(10) NOT NULL,
	"NumericCode" nvarchar2(3) NULL,
	"Alpha3Code" nvarchar2(3) NULL,
	"Fips10Code" nvarchar2(2) NULL,
	"Type" nvarchar2(20) NULL, -- e.g. "" or "subdivision"
	"IdStatus" nvarchar2(20) NULL,
	"CurrencyCode" nvarchar2(3) NULL,
	"ParentCode" nvarchar2(10) NULL,
	"ParentGroupCodes" nvarchar2(100) NULL,
	"LanguageCodes" nvarchar2(100) NULL,
	"LanguageAltCodes" nclob NULL,
        CONSTRAINT "PK_Territories" PRIMARY KEY ("Code")
);


/****** Object:  Table [Currencies] ******/

CREATE TABLE "Currencies"(
	"Code" nvarchar2(3) NOT NULL,
	"NumericCode" nvarchar2(3) NULL,
	"Description" nvarchar2(100) NULL,
	"IdStatus" nvarchar2(20) NULL,
        CONSTRAINT "PK_Currencies" PRIMARY KEY ("Code")
);


/****** Object:  Table [TimeZones] ******/

CREATE TABLE "TimeZones"(
	"Code" nvarchar2(50) NOT NULL,
	"Description" nvarchar2(100) NULL,
	"MetaZoneCode" nvarchar2(50) NULL,
	"TerritoryCode" nvarchar2(10) NULL,
    "StandardOffset" number(5) NULL,
        CONSTRAINT "PK_TimeZones" PRIMARY KEY ("Code")
);


/****** Object:  Table [MetaZones] ******/

CREATE TABLE "MetaZones"(
	"Code" nvarchar2(50) NOT NULL,
        CONSTRAINT "PK_MetaZones" PRIMARY KEY ("Code")
);


/****** Object:  Table [WindowsZones] ******/

CREATE TABLE "WindowsZones"(
	"Code" nvarchar2(50) NOT NULL,
	"TimeZoneCode" nvarchar2(50) NOT NULL,
	"TerritoryCode" nvarchar2(10) NOT NULL,
        CONSTRAINT "PK_WindowsZones" PRIMARY KEY ("Code", "TimeZoneCode", "TerritoryCode")
);


/****** Locale Data ******/

/****** Object:  Table [LanguageNames] ******/

CREATE TABLE "LanguageNames"(
	"Code" nvarchar2(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	"Name" nvarchar2(100) NULL,
	"ShortName" nvarchar2(50) NULL,
	"LongName" nvarchar2(100) NULL,
	"VariantName" nvarchar2(100) NULL,
	"SecondaryName" nvarchar2(100) NULL,
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_LanguageNames" PRIMARY KEY ("Locale", "Code")
);


/****** Object:  Table [ScriptNames] ******/

CREATE TABLE "ScriptNames"(
	"Code" nvarchar2(4) NOT NULL,
	"Name" nvarchar2(100) NULL,
	"ShortName" nvarchar2(50) NULL,
	"VariantName" nvarchar2(100) NULL,
	"StandAloneName" nvarchar2(100) NULL,
	"SecondaryName" nvarchar2(100) NULL,
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_ScriptNames" PRIMARY KEY ("Locale", "Code")
);


/****** Object:  Table [TerritoryNames] ******/

CREATE TABLE "TerritoryNames"(
	"Code" nvarchar2(10) NOT NULL,
	"Name" nvarchar2(100) NULL,
	"ShortName" nvarchar2(50) NULL,
	"VariantName" nvarchar2(100) NULL,
	"Type" nvarchar2(20) NULL, -- e.g. "" or set as "subdivision"
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_TerritoryNames" PRIMARY KEY ("Locale", "Code")
);


/****** Object:  Table [CurrencyNames] ******/

CREATE TABLE "CurrencyNames"(
	"Code" nvarchar2(3) NOT NULL,
	"Name" nvarchar2(100) NULL,
	"Symbol" nvarchar2(20) NULL,
	"SymbolNarrow" nvarchar2(20) NULL, -- if symbol alt="narrow" exists
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_CurrencyNames" PRIMARY KEY ("Locale", "Code")
);


/****** Object:  Table [TimeZoneNames] ******/

CREATE TABLE "TimeZoneNames"(
	"Code" nvarchar2(50) NOT NULL,
	"Name" nvarchar2(255) NULL,
	"City" nvarchar2(255) NULL,
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_TimeZoneNames" PRIMARY KEY ("Locale", "Code")
);


/****** Object:  Table [MetaZoneNames] ******/

CREATE TABLE "MetaZoneNames"(
	"Code" nvarchar2(50) NOT NULL,
	"Name" nvarchar2(255) NULL,
	"ShortName" nvarchar2(20) NULL,
	"Locale" nvarchar2(20) NOT NULL,
        CONSTRAINT "PK_MetaZoneNames" PRIMARY KEY ("Locale", "Code")
);
