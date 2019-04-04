/****** Object:  View [dbo].[LatestApprovedGameVersionForEachRegulation]    Script Date: 28/10/2016 11:45:17 ******/
DROP VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
GO

ALTER TABLE dbo.GameVersion_Regulation_ClientType
	DROP CONSTRAINT UniqueClientTypePerGameVersionAndRegulation
GO
ALTER TABLE dbo.GameVersion_Regulation_ClientType ADD CONSTRAINT
	UniqueClientTypePerGameVersionAndRegulation UNIQUE NONCLUSTERED 
	(
	GameVersionRegulationClient_ID,
	ClientType_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX IX_GameVersion_Language_LanguageHash ON dbo.GameVersion_Language
	(
	LanguageHash
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_GameVersion_Property_History_GameVersion_ID ON dbo.GameVersion_Property_History
	(
	GameVersion_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO