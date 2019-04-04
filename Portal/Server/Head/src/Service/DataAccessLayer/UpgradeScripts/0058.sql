CREATE VIEW [dbo].[LastQAApprovedVersionForEachGame]
AS
SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, MAX(dbo.GameVersion.VersionAsLong) 
                         AS LatestQAApprovedVersion
FROM            dbo.GameVersion INNER JOIN
                         dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
WHERE        (dbo.GameVersion_Regulation.QAApprovalDate IS NOT NULL)
GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType

GO


ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT        dbo.GameInfrastructure.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.LastVersionForEachGame.LatestVersion, dbo.Game.IsExternal, 
                         dbo.GameInfrastructure.Technology, dbo.GameInfrastructure.PlatformType, dbo.RegulationType.RegulationType_ID, dbo.GameInfrastructure.Regulation, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.GameVersion_ID AS QA_VersionID, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.LastVersion AS QA_Version, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.DownloadUri AS QA_DownloadUri, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.FileName AS QA_FileName, dbo.LatestQAApprovedGameVersionForEachRegulation.FileSize AS QA_FileSize, 
                         dbo.LatestQAApprovedGameVersionForEachRegulation.MD5 AS QA_MD5, dbo.LatestPMApprovedGameVersionForEachRegulation.GameVersion_ID AS PM_VersionID,
                          dbo.LatestPMApprovedGameVersionForEachRegulation.LastVersion AS PM_Version, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.DownloadUri AS PM_DownloadUri, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.FileName AS PM_FileName, 
                         dbo.LatestPMApprovedGameVersionForEachRegulation.FileSize AS PM_FileSize, dbo.LatestPMApprovedGameVersionForEachRegulation.MD5 AS PM_MD5, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.GameVersion_ID AS PROD_VersionID, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.LastVersion AS PROD_Version, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.DownloadUri AS PROD_DownloadUri, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.FileName AS PROD_FileName, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.FileSize AS PROD_FileSize, 
                         dbo.LatestProductionUploadGameVersionForEachRegulation.MD5 AS PROD_MD5, dbo.LastQAApprovedVersionForEachGame.LatestQAApprovedVersion
FROM            dbo.GameInfrastructure INNER JOIN
                         dbo.LastVersionForEachGame ON dbo.GameInfrastructure.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LastVersionForEachGame.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LastVersionForEachGame.PlatformType INNER JOIN
                         dbo.Game ON dbo.GameInfrastructure.Game_ID = dbo.Game.Game_ID INNER JOIN
                         dbo.RegulationType ON dbo.GameInfrastructure.Regulation = dbo.RegulationType.RegulationName LEFT OUTER JOIN
                         dbo.LastQAApprovedVersionForEachGame ON dbo.GameInfrastructure.Game_ID = dbo.LastQAApprovedVersionForEachGame.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LastQAApprovedVersionForEachGame.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LastQAApprovedVersionForEachGame.PlatformType LEFT OUTER JOIN
                         dbo.LatestProductionUploadGameVersionForEachRegulation ON 
                         dbo.GameInfrastructure.Game_ID = dbo.LatestProductionUploadGameVersionForEachRegulation.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestProductionUploadGameVersionForEachRegulation.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestProductionUploadGameVersionForEachRegulation.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestProductionUploadGameVersionForEachRegulation.Regulation LEFT OUTER JOIN
                         dbo.LatestQAApprovedGameVersionForEachRegulation ON dbo.GameInfrastructure.Game_ID = dbo.LatestQAApprovedGameVersionForEachRegulation.Game_ID AND 
                         dbo.GameInfrastructure.Technology = dbo.LatestQAApprovedGameVersionForEachRegulation.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestQAApprovedGameVersionForEachRegulation.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestQAApprovedGameVersionForEachRegulation.Regulation LEFT OUTER JOIN
                         dbo.LatestPMApprovedGameVersionForEachRegulation ON dbo.GameInfrastructure.Game_ID = dbo.LatestPMApprovedGameVersionForEachRegulation.Game_ID AND
                          dbo.GameInfrastructure.Technology = dbo.LatestPMApprovedGameVersionForEachRegulation.Technology AND 
                         dbo.GameInfrastructure.PlatformType = dbo.LatestPMApprovedGameVersionForEachRegulation.PlatformType AND 
                         dbo.GameInfrastructure.Regulation = dbo.LatestPMApprovedGameVersionForEachRegulation.Regulation
WHERE        (dbo.LatestQAApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL) OR
                         (dbo.LatestPMApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL) OR
                         (dbo.LatestProductionUploadGameVersionForEachRegulation.LastVersion IS NOT NULL)

GO

