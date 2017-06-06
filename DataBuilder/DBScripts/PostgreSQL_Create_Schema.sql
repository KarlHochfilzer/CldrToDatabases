-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Schema "Countries", for owner 'postgres'
--               This will not create your DB!! DB needs ENCODING = 'UTF8'!!
--               Used DB from connection-string!!

-- =============================================
-- Script to create Schema and Tables

-- 1) If the Schema exists then drop and create new Schema.

-- SCHEMA: "Countries"

DROP SCHEMA IF EXISTS "Countries" CASCADE;

CREATE SCHEMA "Countries" AUTHORIZATION postgres;

COMMENT ON SCHEMA "Countries" IS 'Schema to store Countries data.';

GRANT ALL ON SCHEMA "Countries" TO postgres;

GRANT ALL ON SCHEMA "Countries" TO PUBLIC;

-- No need to always prepend the schemaname!
SET search_path TO "Countries";

/****** Object:  Table [Languages] ******/

CREATE TABLE "Languages"
(
    "Code" character varying(10) COLLATE pg_catalog."default" NOT NULL,
    "IdStatus" character varying(20) COLLATE pg_catalog."default",
    "ParentCode" character varying(10) COLLATE pg_catalog."default",
    CONSTRAINT "Languages_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "Languages"
    OWNER to postgres;

/****** Object:  Table [Scripts] ******/

CREATE TABLE "Scripts"
(
    "Code" character varying(5) COLLATE pg_catalog."default" NOT NULL,
    "IdStatus" character varying(20) COLLATE pg_catalog."default",
    "LanguageCodes" bytea,
    "LanguageAltCodes" bytea,
    CONSTRAINT "Scripts_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "Scripts"
    OWNER to postgres;

/****** Object:  Table [Territories] ******/

CREATE TABLE "Territories"
(
    "Code" character varying(10) COLLATE pg_catalog."default" NOT NULL,
    "NumericCode" character varying(3) COLLATE pg_catalog."default",
    "Alpha3Code" character varying(3) COLLATE pg_catalog."default",
    "Fips10Code" character varying(2) COLLATE pg_catalog."default",
    "Type" character varying(20) COLLATE pg_catalog."default",
    "IdStatus" character varying(20) COLLATE pg_catalog."default",
    "CurrencyCode" character varying(3) COLLATE pg_catalog."default",
    "ParentCode" character varying(10) COLLATE pg_catalog."default",
    "ParentGroupCodes" character varying(100) COLLATE pg_catalog."default",
    "LanguageCodes" character varying(100) COLLATE pg_catalog."default",
    "LanguageAltCodes" bytea,
    CONSTRAINT "Territories_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "Territories"
    OWNER to postgres;

/****** Object:  Table [Currencies] ******/
CREATE TABLE "Currencies"
(
    "Code" character varying(4) COLLATE pg_catalog."default" NOT NULL,
    "NumericCode" character varying(3) COLLATE pg_catalog."default",
    "Description" character varying(100) COLLATE pg_catalog."default",
    "IdStatus" character varying(20) COLLATE pg_catalog."default",
    CONSTRAINT "Currencies_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "Currencies"
    OWNER to postgres;

/****** Object:  Table [TimeZones] ******/
CREATE TABLE "TimeZones"
(
    "Code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Description" character varying(100) COLLATE pg_catalog."default",
    "MetaZoneCode" character varying(50) COLLATE pg_catalog."default",
    "TerritoryCode" character varying(10) COLLATE pg_catalog."default",
    "StandardOffset" integer,
    CONSTRAINT "TimeZones_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "TimeZones"
    OWNER to postgres;

/****** Object:  Table [MetaZones] ******/
CREATE TABLE "MetaZones"
(
    "Code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "MetaZones_pkey" PRIMARY KEY ("Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "MetaZones"
    OWNER to postgres;

/****** Object:  Table [WindowsZones] ******/
CREATE TABLE "WindowsZones"
(
    "Code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "TimeZoneCode" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "TerritoryCode" character varying(10) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "WindowsZones_pkey" PRIMARY KEY ("Code", "TimeZoneCode", "TerritoryCode")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "WindowsZones"
    OWNER to postgres;

/****** Locale Data ******/

/****** Object:  Table [LanguageNames] ******/
CREATE TABLE "LanguageNames"
(
    "Code" character varying(10) COLLATE pg_catalog."default" NOT NULL,
	"Name" character varying(100) COLLATE pg_catalog."default",
	"ShortName" character varying(50) COLLATE pg_catalog."default",
	"LongName" character varying(100) COLLATE pg_catalog."default",
	"VariantName" character varying(100) COLLATE pg_catalog."default",
	"SecondaryName" character varying(100) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "LanguageNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "LanguageNames"
    OWNER to postgres;

/****** Object:  Table [ScriptNames] ******/
CREATE TABLE "ScriptNames"
(
    "Code" character varying(5) COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying(100) COLLATE pg_catalog."default",
	"ShortName" character varying(50) COLLATE pg_catalog."default",
	"VariantName" character varying(100) COLLATE pg_catalog."default",
	"StandAloneName" character varying(100) COLLATE pg_catalog."default",
	"SecondaryName" character varying(100) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "ScriptNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "ScriptNames"
    OWNER to postgres;

/****** Object:  Table [TerritoryNames] ******/
CREATE TABLE "TerritoryNames"
(
    "Code" character varying(10) COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying(100) COLLATE pg_catalog."default",
	"ShortName" character varying(50) COLLATE pg_catalog."default",
	"VariantName" character varying(100) COLLATE pg_catalog."default",
    "Type" character varying(20) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "TerritoryNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "TerritoryNames"
    OWNER to postgres;

/****** Object:  Table [CurrencyNames] ******/
CREATE TABLE "CurrencyNames"
(
    "Code" character varying(4) COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying(100) COLLATE pg_catalog."default",
    "Symbol" character varying(20) COLLATE pg_catalog."default",
    "SymbolNarrow" character varying(20) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "CurrencyNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "CurrencyNames"
    OWNER to postgres;

/****** Object:  Table [TimeZoneNames] ******/
CREATE TABLE "TimeZoneNames"
(
    "Code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying(255) COLLATE pg_catalog."default",
    "City" character varying(255) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "TimeZoneNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "TimeZoneNames"
    OWNER to postgres;

/****** Object:  Table [MetaZoneNames] ******/
CREATE TABLE "MetaZoneNames"
(
    "Code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying(255) COLLATE pg_catalog."default",
    "ShortName" character varying(20) COLLATE pg_catalog."default",
    "Locale" character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "MetaZoneNames_pkey" PRIMARY KEY ("Locale", "Code")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE "MetaZoneNames"
    OWNER to postgres;
