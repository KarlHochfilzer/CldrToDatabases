-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Database 'Countries'

-- =============================================
-- Script to create DB and Tables
USE MASTER;   
-- 1) Check for the Database Exists. If the database exists then drop and create new DB.
IF EXISTS (SELECT [name] FROM sys.databases WHERE [name] = 'Countries' )   
BEGIN   
ALTER DATABASE Countries SET SINGLE_USER WITH ROLLBACK IMMEDIATE   
DROP DATABASE Countries ;   
END     
   
CREATE DATABASE Countries
GO   

USE [Countries]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Table [dbo].[Languages] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Languages](
	[Code] [nvarchar](10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	[IdStatus] [nvarchar](20) NULL,
	[ParentCode] [nvarchar](10) NULL
 CONSTRAINT [PK_dbo.Languages] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Scripts] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Scripts](
	[Code] [nvarchar](4) NOT NULL,
	[IdStatus] [nvarchar](20) NULL,
	[LanguageCodes] [nvarchar](max) NULL,
	[LanguageAltCodes] [nvarchar](max) NULL
 CONSTRAINT [PK_dbo.Scripts] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Territories] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Territories](
	[Code] [nvarchar](10) NOT NULL,
	[NumericCode] [nvarchar](3) NULL,
	[Alpha3Code] [nvarchar](3) NULL,
	[Fips10Code] [nvarchar](2) NULL,
	[Type] [nvarchar](20) NULL, -- e.g. "" or "subdivision"
	[IdStatus] [nvarchar](20) NULL,
	[CurrencyCode] [nvarchar](3) NULL,
	[ParentCode] [nvarchar](10) NULL,
	[ParentGroupCodes] [nvarchar](100) NULL,
	[LanguageCodes] [nvarchar](100) NULL,
	[LanguageAltCodes] [nvarchar](max) NULL
 CONSTRAINT [PK_dbo.Territories] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Currencies] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Currencies](
	[Code] [nvarchar](3) NOT NULL,
	[NumericCode] [nvarchar](3) NULL,
	[Description] [nvarchar](100) NULL,
	[IdStatus] [nvarchar](20) NULL
 CONSTRAINT [PK_dbo.Currencies] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[TimeZones] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[TimeZones](
	[Code] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[MetaZoneCode] [nvarchar](50) NULL,
	[TerritoryCode] [nvarchar](10) NULL,
    [StandardOffset] [int] NULL
 CONSTRAINT [PK_dbo.TimeZones] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[MetaZones] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[MetaZones](
	[Code] [nvarchar](50) NOT NULL
 CONSTRAINT [PK_dbo.MetaZones] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[WindowsZones] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[WindowsZones](
	[Code] [nvarchar](50) NOT NULL,
	[TimeZoneCode] [nvarchar](50) NOT NULL,
	[TerritoryCode] [nvarchar](10) NOT NULL
 CONSTRAINT [PK_dbo.WindowsZones] PRIMARY KEY CLUSTERED 
(
	[Code] ASC,
	[TimeZoneCode] ASC,
    [TerritoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Locale Data ******/

/****** Object:  Table [dbo].[LanguageNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[LanguageNames](
	[Code] [nvarchar](10) NOT NULL, -- ISO_639-3, ISO_639-2, ISO_639-1 or lng-country
	[Name] [nvarchar](100) NULL,
	[ShortName] [nvarchar](50) NULL,
	[LongName] [nvarchar](100) NULL,
	[VariantName] [nvarchar](100) NULL,
	[SecondaryName] [nvarchar](100) NULL,
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.LanguageNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[ScriptNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[ScriptNames](
	[Code] [nvarchar](4) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[ShortName] [nvarchar](50) NULL,
	[VariantName] [nvarchar](100) NULL,
	[StandAloneName] [nvarchar](100) NULL,
	[SecondaryName] [nvarchar](100) NULL,
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.ScriptNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[TerritoryNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[TerritoryNames](
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[ShortName] [nvarchar](50) NULL,
	[VariantName] [nvarchar](100) NULL,
	[Type] [nvarchar](20) NULL, -- e.g. "" or set as "subdivision"
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.TerritoryNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[CurrencyNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[CurrencyNames](
	[Code] [nvarchar](3) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Symbol] [nvarchar](20) NULL,
	[SymbolNarrow] [nvarchar](20) NULL, -- if symbol alt="narrow" exists
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.CurrencyNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[TimeZoneNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[TimeZoneNames](
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.TimeZoneNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[MetaZoneNames] ******/
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[MetaZoneNames](
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[ShortName] [nvarchar](20) NULL,
	[Locale] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_dbo.MetaZoneNames] PRIMARY KEY CLUSTERED 
(
	[Locale] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_PADDING ON
GO

/****** Foreign Keys ParentCode ******/

/****** Object:  Index [IX_LanguagesParentCode] ******/
CREATE NONCLUSTERED INDEX [IX_LanguagesParentCode] ON [dbo].[Languages]
(
	[ParentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[Languages]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Languages_dbo.Languages_ParentCode] FOREIGN KEY([ParentCode])
REFERENCES [dbo].[Languages] ([Code])
ON DELETE NO ACTION
GO
/* Microsoft doesn't support ON DELETE SET NULL for self-referencing foreign key Constraints */
/* Look at: https://connect.microsoft.com/SQLServer/Feedback/Details/3121096 */
/* So needed to add triggers to do this! */

/****** Object:  Index [IX_TerritoriesParentCode] ******/
CREATE NONCLUSTERED INDEX [IX_TerritoriesParentCode] ON [dbo].[Territories]
(
	[ParentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[Territories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Territories_dbo.Territories_ParentCode] FOREIGN KEY([ParentCode])
REFERENCES [dbo].[Territories] ([Code])
ON DELETE NO ACTION
GO
/* Microsoft doesn't support ON DELETE SET NULL for self-referencing foreign key Constraints */
/* Look at: https://connect.microsoft.com/SQLServer/Feedback/Details/3121096 */
/* So needed to add triggers to do this! */


/****** Foreign Keys other table ******/

/****** Object:  Index [IX_TerritoriesCurrencyCode] ******/
CREATE NONCLUSTERED INDEX [IX_TerritoriesCurrencyCode] ON [dbo].[Territories]
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[Territories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Territories_dbo.Currencies_Code] FOREIGN KEY([CurrencyCode])
REFERENCES [dbo].[Currencies] ([Code])
ON DELETE SET NULL
GO

/****** Object:  Index [IX_TimeZonesMetaZoneCode] ******/
CREATE NONCLUSTERED INDEX [IX_TimeZonesMetaZoneCode] ON [dbo].[TimeZones]
(
	[MetaZoneCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[TimeZones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TimeZones_dbo.MetaZones_Code] FOREIGN KEY([MetaZoneCode])
REFERENCES [dbo].[MetaZones] ([Code])
ON DELETE SET NULL
GO

/****** Object:  Index [IX_TimeZonesTerritoryCode] ******/
CREATE NONCLUSTERED INDEX [IX_TimeZonesTerritoryCode] ON [dbo].[TimeZones]
(
	[TerritoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[TimeZones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TimeZones_dbo.Territories_Code] FOREIGN KEY([TerritoryCode])
REFERENCES [dbo].[Territories] ([Code])
ON DELETE SET NULL
GO

/****** Object:  Index [IX_WindowsZonesTimeZoneCode] ******/
CREATE NONCLUSTERED INDEX [IX_WindowsZonesTimeZoneCode] ON [dbo].[WindowsZones]
(
	[TimeZoneCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[WindowsZones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WindowsZones_dbo.TimeZones_Code] FOREIGN KEY([TimeZoneCode])
REFERENCES [dbo].[TimeZones] ([Code])
ON DELETE CASCADE
GO

/****** Object:  Index [IX_WindowsZonesTerritoryCode] ******/
CREATE NONCLUSTERED INDEX [IX_WindowsZonesTerritoryCode] ON [dbo].[WindowsZones]
(
	[TerritoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[WindowsZones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WindowsZones_dbo.Territories_Code] FOREIGN KEY([TerritoryCode])
REFERENCES [dbo].[Territories] ([Code])
ON DELETE CASCADE
GO
