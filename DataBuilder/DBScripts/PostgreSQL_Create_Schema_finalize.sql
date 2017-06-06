-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Keys on Schema "Countries"

-- =============================================
-- Script to create keys

-- No need to always prepend the schemaname!
SET search_path TO "Countries";


/****** Foreign Keys ParentCode ******/

/****** Object:  Constraints [FK_Languages_Languages_ParentCode] ******/
ALTER TABLE "Languages"  ADD CONSTRAINT "FK_Languages_Languages_ParentCode" FOREIGN KEY("ParentCode")
REFERENCES "Languages" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints [FK_Territories_Territories_ParentCode] ******/
ALTER TABLE "Territories"  ADD CONSTRAINT "FK_Territories_Territories_ParentCode" FOREIGN KEY("ParentCode")
REFERENCES "Territories" ("Code")
ON DELETE SET NULL;


/****** Foreign Keys other table ******/

/****** Object:  Constraints [FK_Territories_Currencies_Code] ******/
ALTER TABLE "Territories"  ADD CONSTRAINT "FK_Territories_Currencies_Code" FOREIGN KEY("CurrencyCode")
REFERENCES "Currencies" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints [FK_TimeZones_MetaZones_Code] ******/
ALTER TABLE "TimeZones"  ADD CONSTRAINT "FK_TimeZones_MetaZones_Code" FOREIGN KEY("MetaZoneCode")
REFERENCES "MetaZones" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints [FK_TimeZones_Territories_Code] ******/
ALTER TABLE "TimeZones"  ADD CONSTRAINT "FK_TimeZones_Territories_Code" FOREIGN KEY("TerritoryCode")
REFERENCES "Territories" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints [FK_WindowsZones_TimeZones_Code] ******/
ALTER TABLE "WindowsZones"  ADD CONSTRAINT "FK_WindowsZones_TimeZones_Code" FOREIGN KEY("TimeZoneCode")
REFERENCES "TimeZones" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_WindowsZones_Territories_Code] ******/
ALTER TABLE "WindowsZones"  ADD CONSTRAINT "FK_WindowsZones_Territories_Code" FOREIGN KEY("TerritoryCode")
REFERENCES "Territories" ("Code")
ON DELETE CASCADE;


/****** Foreign Keys, Locale Data ******/

/****** Object:  Constraints [FK_LanguageNames_Languages_Code] ******/
ALTER TABLE "LanguageNames"  ADD CONSTRAINT "FK_LanguageNames_Languages_Code" FOREIGN KEY("Code")
REFERENCES "Languages" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_ScriptNames_Scripts_Code] ******/
ALTER TABLE "ScriptNames"  ADD CONSTRAINT "FK_ScriptNames_Scripts_Code" FOREIGN KEY("Code")
REFERENCES "Scripts" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_TerritoryNames_Territories_Code] ******/
ALTER TABLE "TerritoryNames"  ADD CONSTRAINT "FK_TerritoryNames_Territories_Code" FOREIGN KEY("Code")
REFERENCES "Territories" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_CurrencyNames_Currencies_Code] ******/
ALTER TABLE "CurrencyNames"  ADD CONSTRAINT "FK_CurrencyNames_Currencies_Code" FOREIGN KEY("Code")
REFERENCES "Currencies" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_TimeZoneNames_TimeZones_Code] ******/
ALTER TABLE "TimeZoneNames"  ADD CONSTRAINT "FK_TimeZoneNames_TimeZones_Code" FOREIGN KEY("Code")
REFERENCES "TimeZones" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints [FK_MetaZoneNames_MetaZones_Code] ******/
ALTER TABLE "MetaZoneNames"  ADD CONSTRAINT "FK_MetaZoneNames_MetaZones_Code" FOREIGN KEY("Code")
REFERENCES "MetaZones" ("Code")
ON DELETE CASCADE;
