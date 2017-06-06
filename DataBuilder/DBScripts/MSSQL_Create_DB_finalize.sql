-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create Keys on 'Countries'-tables

-- =============================================
-- Script to create keys

USE [Countries]
GO

/****** Foreign Keys ParentCode ******/

/****** Check Constraint ******/
ALTER TABLE [dbo].[Languages] CHECK CONSTRAINT [FK_dbo.Languages_dbo.Languages_ParentCode]
GO

/****** Check Constraint ******/
ALTER TABLE [dbo].[Territories] CHECK CONSTRAINT [FK_dbo.Territories_dbo.Territories_ParentCode]
GO


/****** Foreign Keys other table ******/

/****** Check Constraint ******/
ALTER TABLE [dbo].[Territories] CHECK CONSTRAINT [FK_dbo.Territories_dbo.Currencies_Code]
GO

/****** Check Constraint ******/
ALTER TABLE [dbo].[TimeZones] CHECK CONSTRAINT [FK_dbo.TimeZones_dbo.MetaZones_Code]
GO

/****** Check Constraint ******/
ALTER TABLE [dbo].[TimeZones] CHECK CONSTRAINT [FK_dbo.TimeZones_dbo.Territories_Code]
GO

/****** Check Constraint ******/
ALTER TABLE [dbo].[WindowsZones] CHECK CONSTRAINT [FK_dbo.WindowsZones_dbo.TimeZones_Code]
GO

/****** Check Constraint ******/
ALTER TABLE [dbo].[WindowsZones] CHECK CONSTRAINT [FK_dbo.WindowsZones_dbo.Territories_Code]
GO


/****** Foreign Keys, Locale Data ******/

/****** Object:  Index [IX_LanguageNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_LanguageNamesCode] ON [dbo].[LanguageNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[LanguageNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LanguageNames_dbo.Languages_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[Languages] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[LanguageNames] CHECK CONSTRAINT [FK_dbo.LanguageNames_dbo.Languages_Code]
GO

/****** Object:  Index [IX_ScriptNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_ScriptNamesCode] ON [dbo].[ScriptNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[ScriptNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ScriptNames_dbo.Scripts_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[Scripts] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[ScriptNames] CHECK CONSTRAINT [FK_dbo.ScriptNames_dbo.Scripts_Code]
GO

/****** Object:  Index [IX_TerritoryNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_TerritoryNamesCode] ON [dbo].[TerritoryNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[TerritoryNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TerritoryNames_dbo.Territories_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[Territories] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[TerritoryNames] CHECK CONSTRAINT [FK_dbo.TerritoryNames_dbo.Territories_Code]
GO

/****** Object:  Index [IX_CurrencyNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_CurrencyNamesCode] ON [dbo].[CurrencyNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[CurrencyNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CurrencyNames_dbo.Currencies_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[Currencies] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[CurrencyNames] CHECK CONSTRAINT [FK_dbo.CurrencyNames_dbo.Currencies_Code]
GO

/****** Object:  Index [IX_TimeZoneNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_TimeZoneNamesCode] ON [dbo].[TimeZoneNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[TimeZoneNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TimeZoneNames_dbo.TimeZones_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[TimeZones] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[TimeZoneNames] CHECK CONSTRAINT [FK_dbo.TimeZoneNames_dbo.TimeZones_Code]
GO

/****** Object:  Index [IX_MetaZoneNamesCode] ******/
CREATE NONCLUSTERED INDEX [IX_MetaZoneNamesCode] ON [dbo].[MetaZoneNames]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Add Constraint ******/
ALTER TABLE [dbo].[MetaZoneNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MetaZoneNames_dbo.MetaZones_Code] FOREIGN KEY([Code])
REFERENCES [dbo].[MetaZones] ([Code])
ON DELETE CASCADE
GO
/****** Check Constraint ******/
ALTER TABLE [dbo].[MetaZoneNames] CHECK CONSTRAINT [FK_dbo.MetaZoneNames_dbo.MetaZones_Code]
GO


/* Microsoft doesn't support ON DELETE SET NULL for self-referencing foreign key Constraints */
/* Look at: https://connect.microsoft.com/SQLServer/Feedback/Details/3121096 */
/* So needed to add triggers to do this! */

/****** Trigger:  Table [dbo].[Languages] ******/

CREATE TRIGGER [Languages_ParentCode_SET_NULL_before_delete_Code]
ON [dbo].[Languages]
INSTEAD OF DELETE
AS 
BEGIN
    UPDATE [dbo].[Languages] SET [ParentCode] = NULL WHERE [ParentCode] IN (SELECT deleted.[Code] from deleted);
    DELETE FROM [dbo].[Languages] WHERE [Code] IN (SELECT deleted.[Code] from deleted);
END
GO

/****** Trigger:  Table [dbo].[Territories] ******/

CREATE TRIGGER [Territories_ParentCode_SET_NULL_before_delete_Code]
ON [dbo].[Territories]
INSTEAD OF DELETE
AS 
BEGIN
    UPDATE [dbo].[Territories] SET [ParentCode] = NULL WHERE [ParentCode] IN (SELECT deleted.[Code] from deleted);
    DELETE FROM [dbo].[Territories] WHERE [Code] IN (SELECT deleted.[Code] from deleted);
END
GO
