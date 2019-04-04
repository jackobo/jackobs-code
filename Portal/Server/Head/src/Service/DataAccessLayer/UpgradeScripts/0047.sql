CREATE TRIGGER AddGameVersionTo_Language_ToArtifactorySyncQueue
   ON  GameVersion_Language
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
INSERT INTO [dbo].[GameVersion_Language_ToArtifactorySyncQueue]
           ([GameVersion_ID])
     
	Select distinct GameVersion_ID From (
    Select [GameVersion_ID] From Inserted
	union 
	Select [GameVersion_ID] From Deleted) allIds
END
GO
