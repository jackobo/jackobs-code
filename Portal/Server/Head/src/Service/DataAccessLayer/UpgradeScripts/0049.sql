ALTER TABLE dbo.GameVersion_Property_History
	DROP CONSTRAINT FK_GameVersion_Property_History_GameVersion
GO
ALTER TABLE dbo.GameVersion_Property_History ADD CONSTRAINT
	FK_GameVersion_Property_History_GameVersion FOREIGN KEY
	(
	GameVersion_ID
	) REFERENCES dbo.GameVersion
	(
	GameVersion_ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO