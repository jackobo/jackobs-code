ALTER VIEW [dbo].[LatestPMApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        PMLatestApproved.Game_ID, PMLatestApproved.LastVersion, PMLatestApproved.Technology, PMLatestApproved.PlatformType, PMLatestApproved.Regulation, 
                         PMLatestApproved.ClientType_ID, GameVersion_1.GameVersion_ID
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation, dbo.GameVersion_Regulation_ClientType.ClientType_ID
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID INNER JOIN
                                                    dbo.GameVersion_Regulation_ClientType ON 
                                                    dbo.GameVersion_Regulation.GameVersionRegulation_ID = dbo.GameVersion_Regulation_ClientType.GameVersionRegulation_ID
                          WHERE        (dbo.GameVersion_Regulation_ClientType.PMApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation, 
                                                    dbo.GameVersion_Regulation_ClientType.ClientType_ID) AS PMLatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON PMLatestApproved.Game_ID = GameVersion_1.Game_ID AND 
                         PMLatestApproved.LastVersion = GameVersion_1.VersionAsLong AND PMLatestApproved.Technology = GameVersion_1.Technology AND 
                         PMLatestApproved.PlatformType = GameVersion_1.PlatformType

GO


ALTER VIEW [dbo].[LatestQAApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        QALatestApproved.Game_ID, QALatestApproved.LastVersion, QALatestApproved.Technology, QALatestApproved.PlatformType, QALatestApproved.Regulation, 
                         QALatestApproved.ClientType_ID, GameVersion_1.GameVersion_ID
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation, dbo.GameVersion_Regulation_ClientType.ClientType_ID
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID INNER JOIN
                                                    dbo.GameVersion_Regulation_ClientType ON 
                                                    dbo.GameVersion_Regulation.GameVersionRegulation_ID = dbo.GameVersion_Regulation_ClientType.GameVersionRegulation_ID
                          WHERE        (dbo.GameVersion_Regulation_ClientType.QAApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation, 
                                                    dbo.GameVersion_Regulation_ClientType.ClientType_ID) AS QALatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON QALatestApproved.LastVersion = GameVersion_1.VersionAsLong AND 
                         QALatestApproved.Technology = GameVersion_1.Technology AND QALatestApproved.PlatformType = GameVersion_1.PlatformType AND 
                         QALatestApproved.Game_ID = GameVersion_1.Game_ID

GO

ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        dbo.GameInfrastructure.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.LastVersionForEachGame.LatestVersion, dbo.Game.IsExternal, 
                         dbo.GameInfrastructure.Technology, dbo.GameInfrastructure.PlatformType, dbo.RegulationType.RegulationType_ID, dbo.GameInfrastructure.Regulation, 
                         dbo.GameInfrastructure.ClientType_ID, dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.GameVersion_ID AS LatestQAApprovedVersion_ID, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.LastVersion AS LatestQAApprovedVersion, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.LastVersion AS LatestPMApprovedVersion, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.GameVersion_ID AS LatestPMApprovedVersion_ID
FROM            dbo.GameInfrastructure INNER JOIN
                         dbo.LastVersionForEachGame ON dbo.GameInfrastructure.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LastVersionForEachGame.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LastVersionForEachGame.PlatformType INNER JOIN
                         dbo.Game ON dbo.GameInfrastructure.Game_ID = dbo.Game.Game_ID INNER JOIN
                         dbo.RegulationType ON dbo.GameInfrastructure.Regulation = dbo.RegulationType.RegulationName LEFT OUTER JOIN
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.Regulation AND 
                         dbo.GameInfrastructure.ClientType_ID = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.ClientType_ID LEFT OUTER JOIN
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Regulation AND 
                         dbo.GameInfrastructure.ClientType_ID = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.ClientType_ID
WHERE        (dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.LastVersion IS NOT NULL) OR
                         (dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.LastVersion IS NOT NULL)

GO

ALTER VIEW [dbo].[NeverApprovedGames]
AS
SELECT        NeverApproved.Game_ID, NeverApproved.GameName, NeverApproved.MainGameType, NeverApproved.IsExternal, NeverApproved.Technology, 
                         NeverApproved.PlatformType, dbo.LastVersionForEachGame.LatestVersion
FROM            (SELECT DISTINCT 
                                                    TOP (100) PERCENT dbo.Game.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.Game.IsExternal, dbo.GameInfrastructure.Technology, 
                                                    dbo.GameInfrastructure.PlatformType
                          FROM            dbo.GameInfrastructure INNER JOIN
                                                    dbo.Game ON dbo.GameInfrastructure.Game_ID = dbo.Game.Game_ID LEFT OUTER JOIN
                                                    dbo.LatestApprovedGameVersionForEachRegulationAndClientType ON 
                                                    dbo.GameInfrastructure.Game_ID = dbo.LatestApprovedGameVersionForEachRegulationAndClientType.Game_ID AND 
                                                    dbo.GameInfrastructure.Technology = dbo.LatestApprovedGameVersionForEachRegulationAndClientType.Technology AND 
                                                    dbo.GameInfrastructure.PlatformType = dbo.LatestApprovedGameVersionForEachRegulationAndClientType.PlatformType
                          WHERE        (dbo.LatestApprovedGameVersionForEachRegulationAndClientType.Game_ID IS NULL)) AS NeverApproved INNER JOIN
                         dbo.LastVersionForEachGame ON NeverApproved.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         NeverApproved.PlatformType = dbo.LastVersionForEachGame.PlatformType AND NeverApproved.Technology = dbo.LastVersionForEachGame.Technology

GO


