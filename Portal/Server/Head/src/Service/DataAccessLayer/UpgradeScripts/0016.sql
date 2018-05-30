sp_rename 'GameVersion_DownloadUri', 'GameVersion_Regulation'
GO
sp_rename 'GamingComponentVersion_DownloadUri', 'GamingComponentVersion_Regulation'
GO

ALTER PROCEDURE [dbo].[GetGamesVersionsAtDate] (@date datetime)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT        Game.MainGameType, Game.GameName, Game.IsExternal, Latest.Technology, GameVersion_1.VersionFolder, GameVersion_1.VersionAsLong, 
                         GameVersion_1.CreatedDate, GameVersion_1.CreatedBy, GameVersion_1.TriggeredBy, GameVersion_Regulation.Regulation, 
                         GameVersion_Regulation.DownloadUri
FROM            (SELECT        Game_ID, Technology, MAX(CreatedDate) AS LastDate
                          FROM            GameVersion
                          WHERE        (CreatedDate < @date)
                          GROUP BY Game_ID, Technology) AS Latest INNER JOIN
                         Game ON Latest.Game_ID = Game.Game_ID INNER JOIN
                         GameVersion AS GameVersion_1 ON Latest.Game_ID = GameVersion_1.Game_ID AND Latest.Technology = GameVersion_1.Technology AND 
                         Latest.LastDate = GameVersion_1.CreatedDate INNER JOIN
                         GameVersion_Regulation ON GameVersion_1.GameVersion_ID = GameVersion_Regulation.GameVersion_ID

order by MainGameType, Regulation
						 

END

GO

