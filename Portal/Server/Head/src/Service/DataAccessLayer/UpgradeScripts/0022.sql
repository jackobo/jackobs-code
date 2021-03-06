﻿CREATE VIEW [dbo].[GameVersion_LanguageApprovalStatus]
AS
SELECT        dbo.GameVersion_Language.GameVersionLanguage_ID, dbo.GameVersion_Language.GameVersion_ID, dbo.GameVersion_Language.Regulation, 
                         dbo.GameVersion_Language.Language, dbo.GameVersion_Language.LanguageHash, CASE WHEN ApprovedLanguage.LanguageHash IS NULL 
                         THEN 0 ELSE 1 END AS IsQAApproved, dbo.GameVersion_Language.IsInProduction, dbo.GameVersion_Language.ArtifactoryLanguage
FROM            dbo.GameVersion_Language LEFT OUTER JOIN
                             (SELECT        LanguageHash
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1
                               WHERE        (IsQAApproved <> 0)
                               GROUP BY LanguageHash) AS ApprovedLanguage ON dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash

GO

