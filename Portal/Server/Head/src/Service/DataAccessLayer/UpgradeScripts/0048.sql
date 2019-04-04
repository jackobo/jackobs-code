ALTER VIEW [dbo].[GameVersion_LanguageQAApprovalInfo]
AS
SELECT DISTINCT dbo.GameVersion_Language.GameVersionLanguage_ID, ApprovedLanguage.QAApprovalDate, ApprovedLanguage.QAApprovalUser
FROM            dbo.GameVersion_Language INNER JOIN
                             (SELECT        GameVersion_Language_1.LanguageHash, GameVersion_Language_1.ArtifactoryLanguage, MAX(GameVersion_Language_1.QAApprovalDate) 
                                                         AS QAApprovalDate, MAX(GameVersion_Language_1.QAApprovalUser) AS QAApprovalUser, dbo.GameVersion.Technology, 
                                                         dbo.GameVersion.PlatformType
                               FROM            dbo.GameVersion_Language AS GameVersion_Language_1 INNER JOIN
                                                         dbo.GameVersion ON GameVersion_Language_1.GameVersion_ID = dbo.GameVersion.GameVersion_ID
                               WHERE        (GameVersion_Language_1.QAApprovalDate IS NOT NULL)
                               GROUP BY GameVersion_Language_1.ArtifactoryLanguage, GameVersion_Language_1.LanguageHash, dbo.GameVersion.Technology, 
                                                         dbo.GameVersion.PlatformType) AS ApprovedLanguage ON 
                         dbo.GameVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage AND 
                         dbo.GameVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON dbo.GameVersion_Language.GameVersion_ID = GameVersion_1.GameVersion_ID AND 
                         ApprovedLanguage.Technology = GameVersion_1.Technology AND ApprovedLanguage.PlatformType = GameVersion_1.PlatformType

GO

CREATE PROCEDURE [dbo].[NormalizeApprovalStatusForAllLanguagesWithTheSameHash]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE GameVersion_Language Set QAApprovalDate = ApprovalInfo.QAApprovalDate, QAApprovalUser = ApprovalInfo.QAApprovalUser
	From GameVersion_Language inner join GameVersion_LanguageQAApprovalInfo ApprovalInfo 
		on GameVersion_Language.GameVersionLanguage_ID = ApprovalInfo.GameVersionLanguage_ID
	where GameVersion_Language.QAApprovalDate is null and ApprovalInfo.QAApprovalDate is not null
END

GO


DROP PROCEDURE [dbo].[UpdateTriggeredByUserNameForAllGameVersions]
GO


