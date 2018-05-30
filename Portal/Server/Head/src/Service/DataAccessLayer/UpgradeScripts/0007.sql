﻿ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT        dbo.Game.Game_ID, dbo.Game.GameName, dbo.LastVersionForEachGame.LastVersion, dbo.LastVersionForEachGame.Technology, dbo.Game.MainGameType, 
                         GameVersion_1.VersionFolder, GameVersion_1.VersionAsLong, LastApprovedVersion.PropertyKey, LastApprovedVersion.PropertyName, 
                         LastApprovedVersion.PropertySet, LastApprovedVersion.Regulation, dbo.Game.IsExternal, GameVersion_1.CreatedDate, GameVersion_1.CreatedBy, 
                         GameVersion_1.GameVersion_ID
FROM            dbo.GameVersion INNER JOIN
                         dbo.Game ON dbo.GameVersion.Game_ID = dbo.Game.Game_ID INNER JOIN
                         dbo.LastVersionForEachGame ON dbo.GameVersion.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         dbo.GameVersion.Technology = dbo.LastVersionForEachGame.Technology AND 
                         dbo.GameVersion.VersionAsLong = dbo.LastVersionForEachGame.LastVersion LEFT OUTER JOIN
                             (SELECT        GameVersion_2.Game_ID, GameVersion_2.Technology, dbo.GameVersion_Property.PropertyKey, dbo.GameVersion_Property.PropertyName, 
                                                         dbo.GameVersion_Property.PropertySet, dbo.GameVersion_Property.Regulation, MAX(GameVersion_2.VersionAsLong) AS LastApprovedVersion
                               FROM            dbo.GameVersion AS GameVersion_2 INNER JOIN
                                                         dbo.GameVersion_Property ON GameVersion_2.GameVersion_ID = dbo.GameVersion_Property.GameVersion_ID
                               WHERE        (dbo.GameVersion_Property.PropertyName = 'State') AND (dbo.GameVersion_Property.PropertyValue IN ('Approved', 'Production', 'Certified')) OR
                                                         (dbo.GameVersion_Property.PropertyName = 'PMApproved') AND (dbo.GameVersion_Property.PropertyValue = N'true')
                               GROUP BY GameVersion_2.Game_ID, GameVersion_2.Technology, dbo.GameVersion_Property.PropertyKey, dbo.GameVersion_Property.PropertyName, 
                                                         dbo.GameVersion_Property.PropertySet, dbo.GameVersion_Property.Regulation) AS LastApprovedVersion INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON LastApprovedVersion.Game_ID = GameVersion_1.Game_ID AND 
                         LastApprovedVersion.LastApprovedVersion = GameVersion_1.VersionAsLong AND LastApprovedVersion.Technology = GameVersion_1.Technology ON 
                         dbo.LastVersionForEachGame.Technology = GameVersion_1.Technology AND dbo.LastVersionForEachGame.Game_ID = GameVersion_1.Game_ID AND 
                         dbo.Game.Game_ID = LastApprovedVersion.Game_ID

GO

CREATE VIEW [dbo].[LatestVersionForEachGameAndRegulation]
AS
SELECT        GameVersion_1.GameVersion_ID, Latest.Game_ID, Latest.Technology, Latest.Regulation, Latest.LastVersion AS VersionAsLong, GameVersion_1.VersionFolder, 
                         dbo.Game.GameName, dbo.Game.MainGameType, dbo.Game.IsExternal
FROM            (SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion_Property.Regulation, MAX(dbo.GameVersion.VersionAsLong) 
                                                    AS LastVersion
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Property ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Property.GameVersion_ID
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion_Property.Regulation) AS Latest INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON Latest.Game_ID = GameVersion_1.Game_ID AND Latest.Technology = GameVersion_1.Technology AND 
                         Latest.LastVersion = GameVersion_1.VersionAsLong INNER JOIN
                         dbo.Game ON GameVersion_1.Game_ID = dbo.Game.Game_ID

GO

