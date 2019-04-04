ALTER TABLE [dbo].[GameVersion_Language] DROP CONSTRAINT [GameVersion_Language_UniqueLanguagePerGameVersionAndRegulation]
GO
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (1 , 'ita')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (2 , 'spa')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (3 , 'eng')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (4 , 'eng')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (6 , 'eng')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (7 , 'eng')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (8 , 'dan')
INSERT INTO [dbo].[RegulationType_MandatoryLanguage] ([RegulationType_ID] ,[LanguageIso3]) VALUES (9 , 'rum')


