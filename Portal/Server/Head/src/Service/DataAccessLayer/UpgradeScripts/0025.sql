CREATE VIEW [dbo].[GameVersion_LanguageApprovalStatus_Verification]
AS
SELECT        dbo.GameVersion_Language.GameVersionLanguage_ID, dbo.GameVersion_Language.GameVersion_ID, dbo.GameVersion_Language.Regulation, 
                         dbo.GameVersion_Language.Language, dbo.GameVersion_Language.LanguageHash, dbo.GameVersion_Language.IsQAApproved AS IsQAApproved1, 
                         dbo.GameVersion_Language.IsInProduction, dbo.GameVersion_Language.ArtifactoryLanguage, 
                         dbo.GameVersion_LanguageApprovalStatus.IsQAApproved AS IsQAApproved2
FROM            dbo.GameVersion_Language INNER JOIN
                         dbo.GameVersion_LanguageApprovalStatus ON 
                         dbo.GameVersion_Language.GameVersionLanguage_ID = dbo.GameVersion_LanguageApprovalStatus.GameVersionLanguage_ID AND 
                         dbo.GameVersion_Language.IsQAApproved <> dbo.GameVersion_LanguageApprovalStatus.IsQAApproved

GO

