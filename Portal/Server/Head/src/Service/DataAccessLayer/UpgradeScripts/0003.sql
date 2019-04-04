ALTER TABLE dbo.GameVersion_Property_History
	DROP CONSTRAINT FK_GameVersion_Property_History_GameVersion

GO

CREATE TABLE dbo.Tmp_GameVersion_Property_History
	(
	Row_ID int NOT NULL IDENTITY (1, 1),
	MainGameType int NOT NULL,
	VersionFolder nchar(10) NULL,
	Regulation nvarchar(50) NOT NULL,
	PropertyKey nvarchar(50) NOT NULL,
	OldValue nvarchar(MAX) NULL,
	NewValue nvarchar(MAX) NULL,
	ChangeDate datetime NOT NULL,
	ChangedBy nvarchar(50) NOT NULL,
	ChangeType int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT dbo.Tmp_GameVersion_Property_History ON
GO
IF EXISTS(SELECT * FROM dbo.GameVersion_Property_History)
	 EXEC('INSERT INTO dbo.Tmp_GameVersion_Property_History (Row_ID, Regulation, PropertyKey, OldValue, NewValue, ChangeDate, ChangedBy, ChangeType)
		SELECT Row_ID, Regulation, PropertyKey, OldValue, NewValue, ChangeDate, ChangedBy, ChangeType FROM dbo.GameVersion_Property_History WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_GameVersion_Property_History OFF
GO
DROP TABLE dbo.GameVersion_Property_History
GO
EXECUTE sp_rename N'dbo.Tmp_GameVersion_Property_History', N'GameVersion_Property_History', 'OBJECT' 
GO
ALTER TABLE dbo.GameVersion_Property_History ADD CONSTRAINT
	PK_GameVersion_Property_History PRIMARY KEY CLUSTERED 
	(
	Row_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

