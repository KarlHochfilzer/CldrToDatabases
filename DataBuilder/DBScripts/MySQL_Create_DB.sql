-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Database 'Countries'

-- befor start:
-- change MySQL-Server 'System Variables' Category 'General/System' Name 'lower_case_table_names=2'
-- to allow uppercase names
-- and append to "C:\ProgramData\MySQL\MySQL Server 5.7\my.ini" (Change to your MySQL Server folder)
-- # allow uppercase names
-- lower_case_table_names=2
-- =============================================
-- Script to create DB and Tables

-- 1) If the database exists then drop and create new DB.

DROP DATABASE IF EXISTS `Countries`;

CREATE DATABASE `Countries`;

USE `Countries`;

/****** Object:  Table [Languages] ******/

CREATE TABLE `Languages`(
	`Code` nvarchar(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	`IdStatus` nvarchar(20) NULL,
	`ParentCode` nvarchar(10) NULL DEFAULT NULL,
     INDEX `IX_Languages_ParentCode` (`ParentCode`),
CONSTRAINT `PK_Languages` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [Scripts] ******/

CREATE TABLE `Scripts`(
	`Code` nvarchar(4) NOT NULL,
	`IdStatus` nvarchar(20) NULL,
	`LanguageCodes` long varbinary NULL,
	`LanguageAltCodes` long varbinary NULL,
 CONSTRAINT `PK_Scripts` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [Territories] ******/

CREATE TABLE `Territories`(
	`Code` nvarchar(10) NOT NULL,
	`NumericCode` nvarchar(3) NULL,
	`Alpha3Code` nvarchar(3) NULL,
	`Fips10Code` nvarchar(2) NULL,
	`Type` nvarchar(20) NULL, -- e.g. "" or "subdivision"
	`IdStatus` nvarchar(20) NULL,
	`CurrencyCode` nvarchar(3) NULL DEFAULT NULL,
     INDEX `IX_Territories_CurrencyCode` (`CurrencyCode`),
	`ParentCode` nvarchar(10) NULL DEFAULT NULL,
     INDEX `IX_Territories_ParentCode` (`ParentCode`),
	`ParentGroupCodes` nvarchar(100) NULL,
	`LanguageCodes` nvarchar(100) NULL,
	`LanguageAltCodes` long varbinary NULL,
 CONSTRAINT `PK_Territories` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [Currencies] ******/

CREATE TABLE `Currencies`(
	`Code` nvarchar(3) NOT NULL,
	`NumericCode` nvarchar(3) NULL,
	`Description` nvarchar(100) NULL,
	`IdStatus` nvarchar(20) NULL,
 CONSTRAINT `PK_Currencies` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [TimeZones] ******/

CREATE TABLE `TimeZones`(
	`Code` nvarchar(50) NOT NULL,
	`Description` nvarchar(100) NULL,
	`MetaZoneCode` nvarchar(50) NULL DEFAULT NULL,
     INDEX `IX_TimeZones_MetaZoneCode` (`MetaZoneCode`),
	`TerritoryCode` nvarchar(10) NULL DEFAULT NULL,
     INDEX `IX_TimeZones_TerritoryCode` (`TerritoryCode`),
    `StandardOffset` int NULL,
 CONSTRAINT `PK_TimeZones` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [MetaZones] ******/

CREATE TABLE `MetaZones`(
	`Code` nvarchar(50) NOT NULL,
 CONSTRAINT `PK_MetaZones` PRIMARY KEY CLUSTERED 
(
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [WindowsZones] ******/

CREATE TABLE `WindowsZones`(
	`Code` nvarchar(50) NOT NULL,
	`TimeZoneCode` nvarchar(50) NOT NULL,
     INDEX `IX_WindowsZones_TimeZoneCode` (`TimeZoneCode`),
	`TerritoryCode` nvarchar(10) NOT NULL,
     INDEX `IX_WindowsZones_TerritoryCode` (`TerritoryCode`),
 CONSTRAINT `PK_WindowsZones` PRIMARY KEY CLUSTERED 
(
	`Code` ASC,
	`TimeZoneCode` ASC,
    `TerritoryCode` ASC
)) ENGINE=INNODB;

/****** Locale Data ******/

/****** Object:  Table [LanguageNames] ******/

CREATE TABLE `LanguageNames`(
	`Code` nvarchar(10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
     INDEX `IX_LanguageNames_Code` (`Code`),
	`Name` nvarchar(100) NULL,
	`ShortName` nvarchar(50) NULL,
	`LongName` nvarchar(100) NULL,
	`VariantName` nvarchar(100) NULL,
	`SecondaryName` nvarchar(100) NULL,
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_LanguageNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [ScriptNames] ******/

CREATE TABLE `ScriptNames`(
	`Code` nvarchar(4) NOT NULL,
     INDEX `IX_ScriptNames_Code` (`Code`),
	`Name` nvarchar(100) NULL,
	`ShortName` nvarchar(50) NULL,
	`VariantName` nvarchar(100) NULL,
	`StandAloneName` nvarchar(100) NULL,
	`SecondaryName` nvarchar(100) NULL,
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_ScriptNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [TerritoryNames] ******/

CREATE TABLE `TerritoryNames`(
	`Code` nvarchar(10) NOT NULL,
     INDEX `IX_TerritoryNames_Code` (`Code`),
	`Name` nvarchar(100) NULL,
	`ShortName` nvarchar(50) NULL,
	`VariantName` nvarchar(100) NULL,
	`Type` nvarchar(20) NULL, -- e.g. "" or set as "subdivision"
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_TerritoryNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [CurrencyNames] ******/

CREATE TABLE `CurrencyNames`(
	`Code` nvarchar(3) NOT NULL,
     INDEX `IX_CurrencyNames_Code` (`Code`),
	`Name` nvarchar(100) NULL,
	`Symbol` nvarchar(20) NULL,
	`SymbolNarrow` nvarchar(20) NULL, -- if symbol alt="narrow" exists
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_CurrencyNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [TimeZoneNames] ******/

CREATE TABLE `TimeZoneNames`(
	`Code` nvarchar(50) NOT NULL,
     INDEX `IX_TimeZoneNames_Code` (`Code`),
	`Name` nvarchar(255) NULL,
	`City` nvarchar(255) NULL,
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_TimeZoneNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;

/****** Object:  Table [MetaZoneNames] ******/

CREATE TABLE `MetaZoneNames`(
	`Code` nvarchar(50) NOT NULL,
     INDEX `IX_MetaZoneNames_Code` (`Code`),
	`Name` nvarchar(255) NULL,
	`ShortName` nvarchar(20) NULL,
	`Locale` nvarchar(20) NOT NULL,
 CONSTRAINT `PK_MetaZoneNames` PRIMARY KEY CLUSTERED 
(
	`Locale` ASC,
	`Code` ASC
)) ENGINE=INNODB;
