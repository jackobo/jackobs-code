ALTER VIEW [dbo].[GameVersion_LanguageQAApprovalInfo]
AS
SELECT DISTINCT dbo.GameVersion_Language.GameVersionLanguage_ID, ApprovedLanguage.QAApprovalDate, ApprovedLanguage.QAApprovalUser
FROM            dbo.GameVersion_Language INNER JOIN
                             (SELECT        LanguageHash, ArtifactoryLanguage, MAX(QAApprovalDate) AS QAApprovalDate, MAX(QAApprovalUser) AS QAApprovalUser
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1
                               WHERE        (QAApprovalDate IS NOT NULL)
                               GROUP BY ArtifactoryLanguage, LanguageHash) AS ApprovedLanguage ON 
                         dbo.GameVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage AND 
                         dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash

GO


