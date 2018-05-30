ALTER VIEW [dbo].[GameVersion_LanguageApprovalStatus]
AS
SELECT DISTINCT 
                         dbo.GameVersion_Language.GameVersionLanguage_ID, dbo.GameVersion_Language.GameVersion_ID, dbo.GameVersion_Language.Regulation, 
                         dbo.GameVersion_Language.Language, dbo.GameVersion_Language.LanguageHash, CONVERT(bit, CASE WHEN ApprovedLanguage.LanguageHash IS NULL 
                         THEN 0 ELSE 1 END) AS IsQAApproved, ApprovedLanguage.QAApprovalDate, ApprovedLanguage.QAApprovalUser, 
                         dbo.GameVersion_Language.ProductionUploadDate, dbo.GameVersion_Language.ArtifactoryLanguage
FROM            dbo.GameVersion_Language LEFT OUTER JOIN
                             (SELECT        LanguageHash, ArtifactoryLanguage, MAX(QAApprovalDate) AS QAApprovalDate, MAX(QAApprovalUser) AS QAApprovalUser
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1
                               WHERE        (QAApprovalDate IS NOT NULL)
                               GROUP BY ArtifactoryLanguage, LanguageHash) AS ApprovedLanguage ON 
                         dbo.GameVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage AND 
                         dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash

GO


