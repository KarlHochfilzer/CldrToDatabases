-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Tables in Database 'Countries'

-- You can not add a foreign key to a table using ALTER TABLE because SQLite does not support ADD CONSTRAINT in the ALTER TABLE statement.

-- =============================================
-- Script to create Tables in existing DB-file

/****** Object:  Table [Languages] ******/

CREATE TABLE [Languages](
	[Code] NVARCHAR(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	[IdStatus] NVARCHAR(20) NULL,
	[ParentCode] NVARCHAR(10) NULL,
        PRIMARY KEY ([Code]),
        FOREIGN KEY ([ParentCode]) REFERENCES [Languages]([Code]) ON DELETE SET NULL
);

/****** Object:  Table [Scripts] ******/

CREATE TABLE [Scripts](
	[Code] NVARCHAR(4) NOT NULL,
	[IdStatus] NVARCHAR(20) NULL,
	[LanguageCodes] NVARCHAR NULL,
	[LanguageAltCodes] NVARCHAR NULL,
        PRIMARY KEY ([Code])
);

/****** Object:  Table [Territories] ******/

CREATE TABLE [Territories](
	[Code] NVARCHAR(10) NOT NULL,
	[NumericCode] NVARCHAR(3) NULL,
	[Alpha3Code] NVARCHAR(3) NULL,
	[Fips10Code] NVARCHAR(2) NULL,
	[Type] NVARCHAR(20) NULL, -- e.g. "" or "subdivision"
	[IdStatus] NVARCHAR(20) NULL,
	[CurrencyCode] NVARCHAR(3) NULL,
	[ParentCode] NVARCHAR(10) NULL,
	[ParentGroupCodes] NVARCHAR(100) NULL,
	[LanguageCodes] NVARCHAR(100) NULL,
	[LanguageAltCodes] NVARCHAR NULL,
        PRIMARY KEY ([Code]),
        FOREIGN KEY ([ParentCode]) REFERENCES [Territories]([Code]) ON DELETE SET NULL,
        FOREIGN KEY ([CurrencyCode]) REFERENCES [Currencies]([Code]) ON DELETE SET NULL
);

/****** Object:  Table [Currencies] ******/

CREATE TABLE [Currencies](
	[Code] NVARCHAR(3) NOT NULL,
	[NumericCode] NVARCHAR(3) NULL,
	[Description] NVARCHAR(100) NULL,
	[IdStatus] NVARCHAR(20) NULL,
        PRIMARY KEY ([Code])
);

/****** Object:  Table [TimeZones] ******/

CREATE TABLE [TimeZones](
	[Code] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(100) NULL,
	[MetaZoneCode] NVARCHAR(50) NULL,
	[TerritoryCode] NVARCHAR(10) NULL,
    [StandardOffset] NUMERIC(5) NULL,
        PRIMARY KEY ([Code]),
        FOREIGN KEY ([MetaZoneCode]) REFERENCES [MetaZones]([Code]) ON DELETE SET NULL,
        FOREIGN KEY ([TerritoryCode]) REFERENCES [Territories]([Code]) ON DELETE SET NULL
);

/****** Object:  Table [MetaZones] ******/

CREATE TABLE [MetaZones](
	[Code] NVARCHAR(50) NOT NULL,
        PRIMARY KEY ([Code])
);

/****** Object:  Table [WindowsZones] ******/

CREATE TABLE [WindowsZones](
	[Code] NVARCHAR(50) NOT NULL,
	[TimeZoneCode] NVARCHAR(50) NOT NULL,
	[TerritoryCode] NVARCHAR(10) NOT NULL,
        PRIMARY KEY ([Code], [TimeZoneCode], [TerritoryCode]),
        FOREIGN KEY ([TimeZoneCode]) REFERENCES [TimeZones]([Code]) ON DELETE CASCADE,
        FOREIGN KEY ([TerritoryCode]) REFERENCES [Territories]([Code]) ON DELETE CASCADE
);

/****** Locale Data ******/

/****** Object:  Table [LanguageNames] ******/

CREATE TABLE [LanguageNames](
	[Code] NVARCHAR(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	[Name] NVARCHAR(100) NULL,
	[ShortName] NVARCHAR(50) NULL,
	[LongName] NVARCHAR(100) NULL,
	[VariantName] NVARCHAR(100) NULL,
	[SecondaryName] NVARCHAR(100) NULL,
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [Languages]([Code]) ON DELETE CASCADE
);

/****** Object:  Table [ScriptNames] ******/

CREATE TABLE [ScriptNames](
	[Code] NVARCHAR(4) NOT NULL,
	[Name] NVARCHAR(100) NULL,
	[ShortName] NVARCHAR(50) NULL,
	[VariantName] NVARCHAR(100) NULL,
	[StandAloneName] NVARCHAR(100) NULL,
	[SecondaryName] NVARCHAR(100) NULL,
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [Scripts]([Code]) ON DELETE CASCADE
);

/****** Object:  Table [TerritoryNames] ******/

CREATE TABLE [TerritoryNames](
	[Code] NVARCHAR(10) NOT NULL,
	[Name] NVARCHAR(100) NULL,
	[ShortName] NVARCHAR(50) NULL,
	[VariantName] NVARCHAR(100) NULL,
	[Type] NVARCHAR(20) NULL, -- e.g. "" or set as "subdivision"
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [Territories]([Code]) ON DELETE CASCADE
);

/****** Object:  Table [CurrencyNames] ******/

CREATE TABLE [CurrencyNames](
	[Code] NVARCHAR(3) NOT NULL,
	[Name] NVARCHAR(100) NULL,
	[Symbol] NVARCHAR(20) NULL,
	[SymbolNarrow] NVARCHAR(20) NULL, -- if symbol alt="narrow" exists
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [Currencies]([Code]) ON DELETE CASCADE
);

/****** Object:  Table [TimeZoneNames] ******/

CREATE TABLE [TimeZoneNames](
	[Code] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(255) NULL,
	[City] NVARCHAR(255) NULL,
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [TimeZones]([Code]) ON DELETE CASCADE
);

/****** Object:  Table [MetaZoneNames] ******/

CREATE TABLE [MetaZoneNames](
	[Code] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(255) NULL,
	[ShortName] NVARCHAR(20) NULL,
	[Locale] NVARCHAR(20) NOT NULL,
        PRIMARY KEY ([Locale], [Code]),
        FOREIGN KEY ([Code]) REFERENCES [MetaZones]([Code]) ON DELETE CASCADE
);
