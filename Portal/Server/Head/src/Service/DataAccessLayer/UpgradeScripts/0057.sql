CREATE VIEW [dbo].[LatestProductionUploadGameVersionForEachRegulation]
AS
SELECT        ProductionLatestUpload.Game_ID, ProductionLatestUpload.LastVersion, ProductionLatestUpload.Technology, ProductionLatestUpload.PlatformType, ProductionLatestUpload.Regulation, 
                         GameVersion_1.GameVersion_ID, GameVersion_Regulation_1.DownloadUri, GameVersion_Regulation_1.FileName, GameVersion_Regulation_1.FileSize, 
                         GameVersion_Regulation_1.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
                          WHERE        (dbo.GameVersion_Regulation.ProductionUploadDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation) 
                         AS ProductionLatestUpload INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON ProductionLatestUpload.Game_ID = GameVersion_1.Game_ID AND 
                         ProductionLatestUpload.LastVersion = GameVersion_1.VersionAsLong AND ProductionLatestUpload.Technology = GameVersion_1.Technology AND 
                         ProductionLatestUpload.PlatformType = GameVersion_1.PlatformType INNER JOIN
                         dbo.GameVersion_Regulation AS GameVersion_Regulation_1 ON GameVersion_1.GameVersion_ID = GameVersion_Regulation_1.GameVersion_ID AND 
                         ProductionLatestUpload.Regulation = GameVersion_Regulation_1.Regulation



GO



ALTER VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT      GameInfrastructure.Game_ID, Game.GameName, Game.MainGameType, LastVersionForEachGame.LatestVersion, Game.IsExternal, GameInfrastructure.Technology, 
                         GameInfrastructure.PlatformType, RegulationType.RegulationType_ID, GameInfrastructure.Regulation, 
                         LatestQAApprovedGameVersionForEachRegulation.GameVersion_ID AS QA_VersionID, 
                         LatestQAApprovedGameVersionForEachRegulation.LastVersion AS QA_Version, LatestQAApprovedGameVersionForEachRegulation.DownloadUri AS QA_DownloadUri,
                          LatestQAApprovedGameVersionForEachRegulation.FileName AS QA_FileName, LatestQAApprovedGameVersionForEachRegulation.FileSize AS QA_FileSize, 
                         LatestQAApprovedGameVersionForEachRegulation.MD5 AS QA_MD5, LatestPMApprovedGameVersionForEachRegulation.GameVersion_ID AS PM_VersionID, 
                         LatestPMApprovedGameVersionForEachRegulation.LastVersion AS PM_Version, 
                         LatestPMApprovedGameVersionForEachRegulation.DownloadUri AS PM_DownloadUri, 
                         LatestPMApprovedGameVersionForEachRegulation.FileName AS PM_FileName, LatestPMApprovedGameVersionForEachRegulation.FileSize AS PM_FileSize, 
                         LatestPMApprovedGameVersionForEachRegulation.MD5 AS PM_MD5, 
                         LatestProductionUploadGameVersionForEachRegulation.GameVersion_ID AS PROD_VersionID, 
                         LatestProductionUploadGameVersionForEachRegulation.LastVersion AS PROD_Version, 
                         LatestProductionUploadGameVersionForEachRegulation.DownloadUri AS PROD_DownloadUri, 
                         LatestProductionUploadGameVersionForEachRegulation.FileName AS PROD_FileName, 
                         LatestProductionUploadGameVersionForEachRegulation.FileSize AS PROD_FileSize, 
                         LatestProductionUploadGameVersionForEachRegulation.MD5 AS PROD_MD5
FROM            GameInfrastructure INNER JOIN
                         LastVersionForEachGame ON GameInfrastructure.Game_ID = LastVersionForEachGame.Game_ID AND 
                         GameInfrastructure.Technology = LastVersionForEachGame.Technology AND GameInfrastructure.PlatformType = LastVersionForEachGame.PlatformType INNER JOIN
                         Game ON GameInfrastructure.Game_ID = Game.Game_ID INNER JOIN
                         RegulationType ON GameInfrastructure.Regulation = RegulationType.RegulationName LEFT OUTER JOIN
                         LatestProductionUploadGameVersionForEachRegulation ON GameInfrastructure.Game_ID = LatestProductionUploadGameVersionForEachRegulation.Game_ID AND 
                         GameInfrastructure.Technology = LatestProductionUploadGameVersionForEachRegulation.Technology AND 
                         GameInfrastructure.PlatformType = LatestProductionUploadGameVersionForEachRegulation.PlatformType AND 
                         GameInfrastructure.Regulation = LatestProductionUploadGameVersionForEachRegulation.Regulation LEFT OUTER JOIN
                         LatestQAApprovedGameVersionForEachRegulation ON GameInfrastructure.Game_ID = LatestQAApprovedGameVersionForEachRegulation.Game_ID AND 
                         GameInfrastructure.Technology = LatestQAApprovedGameVersionForEachRegulation.Technology AND 
                         GameInfrastructure.PlatformType = LatestQAApprovedGameVersionForEachRegulation.PlatformType AND 
                         GameInfrastructure.Regulation = LatestQAApprovedGameVersionForEachRegulation.Regulation LEFT OUTER JOIN
                         LatestPMApprovedGameVersionForEachRegulation ON GameInfrastructure.Game_ID = LatestPMApprovedGameVersionForEachRegulation.Game_ID AND 
                         GameInfrastructure.Technology = LatestPMApprovedGameVersionForEachRegulation.Technology AND 
                         GameInfrastructure.PlatformType = LatestPMApprovedGameVersionForEachRegulation.PlatformType AND 
                         GameInfrastructure.Regulation = LatestPMApprovedGameVersionForEachRegulation.Regulation
WHERE        (LatestQAApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL) OR
                         (LatestPMApprovedGameVersionForEachRegulation.LastVersion IS NOT NULL) OR
                         (LatestProductionUploadGameVersionForEachRegulation.LastVersion IS NOT NULL)
GO

