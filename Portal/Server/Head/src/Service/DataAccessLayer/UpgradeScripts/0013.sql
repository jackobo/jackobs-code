
CREATE TABLE [dbo].[PlatformType](
	[PlatformType_ID] [int] NOT NULL,
	[PlatformName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PlatformType] PRIMARY KEY CLUSTERED 
(
	[PlatformType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[PlatformType] ([PlatformType_ID] ,[PlatformName]) VALUES (1, 'PC')
INSERT INTO [dbo].[PlatformType] ([PlatformType_ID] ,[PlatformName]) VALUES (2, 'Mobile')
INSERT INTO [dbo].[PlatformType] ([PlatformType_ID] ,[PlatformName]) VALUES (3, 'PC & Mobile')

GO

ALTER VIEW [dbo].[LastVersionForEachGame]
AS
SELECT        Game_ID, Technology, PlatformType, MAX(VersionAsLong) AS LastVersion
FROM            dbo.GameVersion
GROUP BY Game_ID, Technology, PlatformType

GO


ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT        Game.Game_ID, Game.GameName, LastVersionForEachGame.LastVersion, LastVersionForEachGame.Technology, Game.MainGameType, 
                         GameVersion.VersionFolder, LastApprovedVersion.PropertyKey, LastApprovedVersion.PropertyName, LastApprovedVersion.PropertySet, 
                         LastApprovedVersion.Regulation, Game.IsExternal, GameVersion.CreatedDate, GameVersion.CreatedBy, GameVersion.GameVersion_ID, 
                         LastApprovedVersion.LastApprovedVersion, RegulationType.RegulationType_ID, LastVersionForEachGame.PlatformType
FROM            Game INNER JOIN
                         LastVersionForEachGame ON Game.Game_ID = LastVersionForEachGame.Game_ID LEFT OUTER JOIN
                             (SELECT        MAX(GameVersion_2.VersionAsLong) AS LastApprovedVersion, GameVersion_2.Technology, GameVersion_2.PlatformType, GameVersion_2.Game_ID, 
                                                         GameVersion_Property.PropertyKey, GameVersion_Property.PropertyName, GameVersion_Property.PropertySet, 
                                                         GameVersion_Property.Regulation
                               FROM            GameVersion AS GameVersion_2 INNER JOIN
                                                         GameVersion_Property ON GameVersion_2.GameVersion_ID = GameVersion_Property.GameVersion_ID
                               WHERE        (GameVersion_Property.PropertyName = 'State') AND (GameVersion_Property.PropertyValue IN ('Approved', 'Production', 'Certified')) OR
                                                         (GameVersion_Property.PropertyName = 'PMApproved') AND (GameVersion_Property.PropertyValue = N'true')
                               GROUP BY GameVersion_2.Game_ID, GameVersion_2.Technology, GameVersion_2.PlatformType, GameVersion_Property.PropertyKey, 
                                                         GameVersion_Property.PropertyName, GameVersion_Property.PropertySet, GameVersion_Property.Regulation) AS LastApprovedVersion INNER JOIN
                         GameVersion AS GameVersion ON LastApprovedVersion.Game_ID = GameVersion.Game_ID AND 
                         LastApprovedVersion.LastApprovedVersion = GameVersion.VersionAsLong AND LastApprovedVersion.Technology = GameVersion.Technology AND 
                         LastApprovedVersion.PlatformType = GameVersion.PlatformType ON LastVersionForEachGame.PlatformType = GameVersion.PlatformType AND 
                         LastVersionForEachGame.Technology = GameVersion.Technology AND LastVersionForEachGame.Game_ID = GameVersion.Game_ID LEFT OUTER JOIN
                         RegulationType ON LastApprovedVersion.Regulation = RegulationType.RegulationName

GO

ALTER VIEW [dbo].[LatestVersionForEachGameAndRegulation]
AS
SELECT        GameVersion_1.GameVersion_ID, Latest.Game_ID, Latest.Technology, Latest.PlatformType, Latest.Regulation, Latest.LastVersion AS VersionAsLong, 
                         GameVersion_1.VersionFolder, dbo.Game.GameName, dbo.Game.MainGameType, dbo.Game.IsExternal
FROM            (SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Property.Regulation, 
                                                    MAX(dbo.GameVersion.VersionAsLong) AS LastVersion
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Property ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Property.GameVersion_ID
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Property.Regulation) 
                         AS Latest INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON Latest.Game_ID = GameVersion_1.Game_ID AND Latest.Technology = GameVersion_1.Technology AND 
                         Latest.LastVersion = GameVersion_1.VersionAsLong AND Latest.PlatformType = GameVersion_1.PlatformType INNER JOIN
                         dbo.Game ON GameVersion_1.Game_ID = dbo.Game.Game_ID

GO



