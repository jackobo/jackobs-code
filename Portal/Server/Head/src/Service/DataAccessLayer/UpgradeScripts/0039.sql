sp_rename 'GameVersion_LanguageApprovalStatus', 'GameVersion_LanguageQAApprovalInfo'

GO

ALTER VIEW [dbo].[GameVersion_LanguageQAApprovalInfo]
AS
SELECT DISTINCT 
                         dbo.GameVersion_Language.GameVersionLanguage_ID, CONVERT(bit, CASE WHEN ApprovedLanguage.LanguageHash IS NULL THEN 0 ELSE 1 END) 
                         AS IsQAApproved, ApprovedLanguage.QAApprovalDate, ApprovedLanguage.QAApprovalUser
FROM            dbo.GameVersion_Language LEFT OUTER JOIN
                             (SELECT        LanguageHash, ArtifactoryLanguage, MAX(QAApprovalDate) AS QAApprovalDate, MAX(QAApprovalUser) AS QAApprovalUser
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1
                               WHERE        (QAApprovalDate IS NOT NULL)
                               GROUP BY ArtifactoryLanguage, LanguageHash) AS ApprovedLanguage ON 
                         dbo.GameVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage AND 
                         dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash

GO


