ALTER TABLE dbo.GameVersion_Language ALTER COLUMN GameVersionLanguage_ID
	ADD ROWGUIDCOL
GO
ALTER TABLE dbo.GameVersion_Language ADD CONSTRAINT
	DF_GameVersion_Language_GameVersionLanguage_ID DEFAULT (newid()) FOR GameVersionLanguage_ID
GO
ALTER TABLE dbo.GameVersion_Language SET (LOCK_ESCALATION = TABLE)
