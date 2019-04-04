sp_rename 'Regulation', 'RegulationsNames'

GO

CREATE TABLE [dbo].[RegulationType](
	[RegulationType_ID] [int] NOT NULL,
	[RegulationName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RegulationType] PRIMARY KEY CLUSTERED 
(
	[RegulationType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT        dbo.Game.Game_ID, dbo.Game.GameName, dbo.LastVersionForEachGame.LastVersion, dbo.LastVersionForEachGame.Technology, dbo.Game.MainGameType, 
                         GameVersion.VersionFolder, LastApprovedVersion.PropertyKey, LastApprovedVersion.PropertyName, LastApprovedVersion.PropertySet, 
                         LastApprovedVersion.Regulation, dbo.Game.IsExternal, GameVersion.CreatedDate, GameVersion.CreatedBy, GameVersion.GameVersion_ID, 
                         LastApprovedVersion.LastApprovedVersion, dbo.RegulationType.RegulationType_ID
FROM            dbo.RegulationType RIGHT OUTER JOIN
                             (SELECT        MAX(GameVersion_2.VersionAsLong) AS LastApprovedVersion, GameVersion_2.Technology, GameVersion_2.Game_ID, 
                                                         dbo.GameVersion_Property.PropertyKey, dbo.GameVersion_Property.PropertyName, dbo.GameVersion_Property.PropertySet, 
                                                         dbo.GameVersion_Property.Regulation
                               FROM            dbo.GameVersion AS GameVersion_2 INNER JOIN
                                                         dbo.GameVersion_Property ON GameVersion_2.GameVersion_ID = dbo.GameVersion_Property.GameVersion_ID
                               WHERE        (dbo.GameVersion_Property.PropertyName = 'State') AND (dbo.GameVersion_Property.PropertyValue IN ('Approved', 'Production', 'Certified')) OR
                                                         (dbo.GameVersion_Property.PropertyName = 'PMApproved') AND (dbo.GameVersion_Property.PropertyValue = N'true')
                               GROUP BY GameVersion_2.Game_ID, GameVersion_2.Technology, dbo.GameVersion_Property.PropertyKey, dbo.GameVersion_Property.PropertyName, 
                                                         dbo.GameVersion_Property.PropertySet, dbo.GameVersion_Property.Regulation) AS LastApprovedVersion INNER JOIN
                         dbo.GameVersion AS GameVersion ON LastApprovedVersion.Game_ID = GameVersion.Game_ID AND 
                         LastApprovedVersion.LastApprovedVersion = GameVersion.VersionAsLong AND LastApprovedVersion.Technology = GameVersion.Technology ON 
                         dbo.RegulationType.RegulationName = LastApprovedVersion.Regulation RIGHT OUTER JOIN
                         dbo.Game INNER JOIN
                         dbo.LastVersionForEachGame ON dbo.Game.Game_ID = dbo.LastVersionForEachGame.Game_ID ON 
                         GameVersion.Technology = dbo.LastVersionForEachGame.Technology AND GameVersion.Game_ID = dbo.LastVersionForEachGame.Game_ID

GO


