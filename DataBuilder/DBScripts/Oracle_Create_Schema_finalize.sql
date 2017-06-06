-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : Script to create constraints on "Countries" Schema objects.

-- Using Express Edition 11.2 defaults.
-- =============================================
-- Script to create constraints

-- No need to always prepend the schema(user)name!
ALTER SESSION SET CURRENT_SCHEMA = "Countries";


/****** Foreign Keys ParentCode ******/

/****** Object:  Constraints ["FK_Lang_Lang_ParentCode"] ******/
ALTER TABLE "Languages"  ADD CONSTRAINT "FK_Lang_Lang_ParentCode" FOREIGN KEY("ParentCode")
REFERENCES "Languages" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints ["FK_Terr_Terr_ParentCode"] ******/
ALTER TABLE "Territories"  ADD CONSTRAINT "FK_Terr_Terr_ParentCode" FOREIGN KEY("ParentCode")
REFERENCES "Territories" ("Code")
ON DELETE SET NULL;


/****** Foreign Keys other table ******/

/****** Object:  Constraints ["FK_Terr_Curr_Code"] ******/
ALTER TABLE "Territories"  ADD CONSTRAINT "FK_Terr_Curr_Code" FOREIGN KEY("CurrencyCode")
REFERENCES "Currencies" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints ["FK_TiZo_MeZo_Code"] ******/
ALTER TABLE "TimeZones"  ADD CONSTRAINT "FK_TiZo_MeZo_Code" FOREIGN KEY("MetaZoneCode")
REFERENCES "MetaZones" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints ["FK_TiZo_Terr_Code"] ******/
ALTER TABLE "TimeZones"  ADD CONSTRAINT "FK_TiZo_Terr_Code" FOREIGN KEY("TerritoryCode")
REFERENCES "Territories" ("Code")
ON DELETE SET NULL;

/****** Object:  Constraints ["FK_WiZo_TiZo_Code"] ******/
ALTER TABLE "WindowsZones"  ADD CONSTRAINT "FK_WiZo_TiZo_Code" FOREIGN KEY("TimeZoneCode")
REFERENCES "TimeZones" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_WiZo_Terr_Code"] ******/
ALTER TABLE "WindowsZones"  ADD CONSTRAINT "FK_WiZo_Terr_Code" FOREIGN KEY("TerritoryCode")
REFERENCES "Territories" ("Code")
ON DELETE CASCADE;


/****** Foreign Keys, Locale Data ******/

/****** Object:  Constraints ["FK_LangNa_Lang_Code"] ******/
ALTER TABLE "LanguageNames"  ADD CONSTRAINT "FK_LangNa_Lang_Code" FOREIGN KEY("Code")
REFERENCES "Languages" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_ScriNa_Scri_Code"] ******/
ALTER TABLE "ScriptNames"  ADD CONSTRAINT "FK_ScriNa_Scri_Code" FOREIGN KEY("Code")
REFERENCES "Scripts" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_TerrNa_Terr_Code"] ******/
ALTER TABLE "TerritoryNames"  ADD CONSTRAINT "FK_TerrNa_Terr_Code" FOREIGN KEY("Code")
REFERENCES "Territories" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_CurrNa_Curr_Code"] ******/
ALTER TABLE "CurrencyNames"  ADD CONSTRAINT "FK_CurrNa_Curr_Code" FOREIGN KEY("Code")
REFERENCES "Currencies" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_TiZoNa_TiZo_Code"] ******/
ALTER TABLE "TimeZoneNames"  ADD CONSTRAINT "FK_TiZoNa_TiZo_Code" FOREIGN KEY("Code")
REFERENCES "TimeZones" ("Code")
ON DELETE CASCADE;

/****** Object:  Constraints ["FK_MeZoNa_MeZo_Code"] ******/
ALTER TABLE "MetaZoneNames"  ADD CONSTRAINT "FK_MeZoNa_MeZo_Code" FOREIGN KEY("Code")
REFERENCES "MetaZones" ("Code")
ON DELETE CASCADE;
