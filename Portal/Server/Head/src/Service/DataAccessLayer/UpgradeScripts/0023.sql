ALTER TABLE dbo.GameVersion_Language_ToArtifactorySyncQueue
	DROP CONSTRAINT DF_GameVersion_LanguageHashQueue_InsertedTime
GO
CREATE TABLE dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue
	(
	Row_ID int NOT NULL IDENTITY (1, 1),
	GameVersion_ID uniqueidentifier NOT NULL,
	InsertedTime datetime2(7) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue ADD CONSTRAINT
	DF_GameVersion_LanguageHashQueue_InsertedTime DEFAULT (getdate()) FOR InsertedTime
GO
SET IDENTITY_INSERT dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue ON
GO
IF EXISTS(SELECT * FROM dbo.GameVersion_Language_ToArtifactorySyncQueue)
	 EXEC('INSERT INTO dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue (Row_ID, InsertedTime)
		SELECT Row_ID, InsertedTime FROM dbo.GameVersion_Language_ToArtifactorySyncQueue WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue OFF
GO
DROP TABLE dbo.GameVersion_Language_ToArtifactorySyncQueue
GO
EXECUTE sp_rename N'dbo.Tmp_GameVersion_Language_ToArtifactorySyncQueue', N'GameVersion_Language_ToArtifactorySyncQueue', 'OBJECT' 
GO
ALTER TABLE dbo.GameVersion_Language_ToArtifactorySyncQueue ADD CONSTRAINT
	PK_GameVersion_LanguageHashQueue PRIMARY KEY CLUSTERED 
	(
	Row_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

