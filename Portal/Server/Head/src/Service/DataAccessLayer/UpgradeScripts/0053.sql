sp_rename 'LatestPMApprovedGameVersionForEachRegulationAndClientType', 'LatestPMApprovedGameVersionForEachRegulation'
GO
sp_rename 'LatestQAApprovedGameVersionForEachRegulationAndClientType', 'LatestQAApprovedGameVersionForEachRegulation'
GO

ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulationAndClientType]
AS
SELECT        dbo.GameInfrastructure.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.LastVersionForEachGame.LatestVersion, dbo.Game.IsExternal, 
                         dbo.GameInfrastructure.Technology, dbo.GameInfrastructure.PlatformType, dbo.RegulationType.RegulationType_ID, dbo.GameInfrastructure.Regulation, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.GameVersion_ID AS QA_VersionID, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.LastVersion AS QA_Version, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.DownloadUri AS QA_DownloadUri, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.FileName AS QA_FileName, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.FileSize AS QA_FileSize, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.MD5 AS QA_MD5, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.GameVersion_ID AS PM_VersionID, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.LastVersion AS PM_Version, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.DownloadUri AS PM_DownloadUri, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.FileName AS PM_FileName, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.FileSize AS PM_FileSize, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.MD5 AS PM_MD5
FROM            dbo.GameInfrastructure INNER JOIN
                         dbo.LastVersionForEachGame ON dbo.GameInfrastructure.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LastVersionForEachGame.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LastVersionForEachGame.PlatformType INNER JOIN
                         dbo.Game ON dbo.GameInfrastructure.Game_ID = dbo.Game.Game_ID INNER JOIN
                         dbo.RegulationType ON dbo.GameInfrastructure.Regulation = dbo.RegulationType.RegulationName LEFT OUTER JOIN
                         dbo.LatestQAApprovedGameVersionForEachRegulation ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestQAApprovedGameVersionForEachRegulation.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestQAApprovedGameVersionForEachRegulation.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestQAApprovedGameVersionForEachRegulation.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestQAApprovedGameVersionForEachRegulation.Regulation LEFT OUTER JOIN
                         dbo.LatestPMApprovedGameVersionForEachRegulation ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestPMApprovedGameVersionForEachRegulation.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestPMApprovedGameVersionForEachRegulation.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestPMApprovedGameVersionForEachRegulation.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestPMApprovedGameVersionForEachRegulation.Regulation
WHERE        (dbo.LatestQAApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL) OR
                         (dbo.LatestPMApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL)


GO

sp_rename 'LatestApprovedGameVersionForEachRegulationAndClientType', 'LatestApprovedGameVersionForEachRegulation'

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
                                                    dbo.LatestApprovedGameVersionForEachRegulation ON 
                                                    dbo.GameInfrastructure.Game_ID = dbo.LatestApprovedGameVersionForEachRegulation.Game_ID AND 
                                                    dbo.GameInfrastructure.Technology = dbo.LatestApprovedGameVersionForEachRegulation.Technology AND 
                                                    dbo.GameInfrastructure.PlatformType = dbo.LatestApprovedGameVersionForEachRegulation.PlatformType
                          WHERE        (dbo.LatestApprovedGameVersionForEachRegulation.Game_ID IS NULL)) AS NeverApproved INNER JOIN
                         dbo.LastVersionForEachGame ON NeverApproved.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         NeverApproved.PlatformType = dbo.LastVersionForEachGame.PlatformType AND NeverApproved.Technology = dbo.LastVersionForEachGame.Technology


GO
