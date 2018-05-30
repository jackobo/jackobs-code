ALTER TABLE dbo.GameVersion_Language ADD
	QAApprovalDate datetime2(7) NULL,
	QAApprovalUser nvarchar(50) NULL,
	ProductionUploadDate datetime2(7) NULL
GO
ALTER TABLE dbo.GameVersion_Language
	DROP CONSTRAINT DF_GameVersion_Language_IsApproved
GO
ALTER TABLE dbo.GameVersion_Language
	DROP CONSTRAINT DF_GameVersion_Language_IsInProduction
GO
ALTER TABLE dbo.GameVersion_Language
	DROP COLUMN IsQAApproved, IsInProduction
GO

DROP VIEW [dbo].[GameVersion_LanguageApprovalStatus_Verification]
GO


ALTER VIEW [dbo].[GameVersion_LanguageApprovalStatus]
AS
SELECT        dbo.GameVersion_Language.GameVersionLanguage_ID, dbo.GameVersion_Language.GameVersion_ID, dbo.GameVersion_Language.Regulation, 
                         dbo.GameVersion_Language.Language, dbo.GameVersion_Language.LanguageHash, CONVERT(bit, CASE WHEN ApprovedLanguage.LanguageHash IS NULL 
                         THEN 0 ELSE 1 END) AS IsQAApproved, dbo.GameVersion_Language.QAApprovalDate, dbo.GameVersion_Language.QAApprovalUser, 
                         dbo.GameVersion_Language.ProductionUploadDate, dbo.GameVersion_Language.ArtifactoryLanguage
FROM            dbo.GameVersion_Language LEFT OUTER JOIN
                             (SELECT        LanguageHash, ArtifactoryLanguage
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1
                               WHERE        (QAApprovalDate IS NOT NULL)
                               GROUP BY ArtifactoryLanguage, LanguageHash) AS ApprovedLanguage ON dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash AND
                          dbo.GameVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage

GO

