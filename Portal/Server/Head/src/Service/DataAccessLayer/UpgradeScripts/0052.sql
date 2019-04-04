/****** Object:  View [dbo].[ChillWrapper_LanguageQAApprovalInfo]    Script Date: 14/02/2017 14:30:02 ******/
DROP VIEW [dbo].[ChillWrapper_LanguageQAApprovalInfo]
GO

ALTER VIEW [dbo].[GameInfrastructure]
AS
SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation
FROM            dbo.GameVersion INNER JOIN
                         dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation

GO

ALTER VIEW [dbo].[LatestQAApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        QALatestApproved.Game_ID, QALatestApproved.LastVersion, QALatestApproved.Technology, QALatestApproved.PlatformType, QALatestApproved.Regulation, 
                         GameVersion_1.GameVersion_ID, GameVersion_Regulation_1.DownloadUri, GameVersion_Regulation_1.FileName, GameVersion_Regulation_1.FileSize, 
                         GameVersion_Regulation_1.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
                          WHERE        (dbo.GameVersion_Regulation.QAApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation) 
                         AS QALatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON QALatestApproved.LastVersion = GameVersion_1.VersionAsLong AND 
                         QALatestApproved.Technology = GameVersion_1.Technology AND QALatestApproved.PlatformType = GameVersion_1.PlatformType AND 
                         QALatestApproved.Game_ID = GameVersion_1.Game_ID INNER JOIN
                         dbo.GameVersion_Regulation AS GameVersion_Regulation_1 ON GameVersion_1.GameVersion_ID = GameVersion_Regulation_1.GameVersion_ID AND 
                         QALatestApproved.Regulation = GameVersion_Regulation_1.Regulation

GO

ALTER VIEW [dbo].[LatestPMApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        PMLatestApproved.Game_ID, PMLatestApproved.LastVersion, PMLatestApproved.Technology, PMLatestApproved.PlatformType, PMLatestApproved.Regulation, 
                         GameVersion_1.GameVersion_ID, GameVersion_Regulation_1.DownloadUri, GameVersion_Regulation_1.FileName, GameVersion_Regulation_1.FileSize, 
                         GameVersion_Regulation_1.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
                          WHERE        (dbo.GameVersion_Regulation.PMApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation) 
                         AS PMLatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON PMLatestApproved.Game_ID = GameVersion_1.Game_ID AND 
                         PMLatestApproved.LastVersion = GameVersion_1.VersionAsLong AND PMLatestApproved.Technology = GameVersion_1.Technology AND 
                         PMLatestApproved.PlatformType = GameVersion_1.PlatformType INNER JOIN
                         dbo.GameVersion_Regulation AS GameVersion_Regulation_1 ON GameVersion_1.GameVersion_ID = GameVersion_Regulation_1.GameVersion_ID AND 
                         PMLatestApproved.Regulation = GameVersion_Regulation_1.Regulation

GO

ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        dbo.GameInfrastructure.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.LastVersionForEachGame.LatestVersion, dbo.Game.IsExternal, 
                         dbo.GameInfrastructure.Technology, dbo.GameInfrastructure.PlatformType, dbo.RegulationType.RegulationType_ID, dbo.GameInfrastructure.Regulation, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.GameVersion_ID AS QA_VersionID, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.LastVersion AS QA_Version, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.DownloadUri AS QA_DownloadUri, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.FileName AS QA_FileName, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.FileSize AS QA_FileSize, 
                         dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.MD5 AS QA_MD5, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.GameVersion_ID AS PM_VersionID, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.LastVersion AS PM_Version, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.DownloadUri AS PM_DownloadUri, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.FileName AS PM_FileName, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.FileSize AS PM_FileSize, 
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.MD5 AS PM_MD5
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
                         dbo.GameInfrastructure.Regulation = dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.Regulation LEFT OUTER JOIN
                         dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.Regulation
WHERE        (dbo.LatestQAApprovedGameVersionForEachRegulationAndClientType.LastVersion IS NOT NULL) OR
                         (dbo.LatestPMApprovedGameVersionForEachRegulationAndClientType.LastVersion IS NOT NULL)

GO

