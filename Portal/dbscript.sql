USE [GamesPortalDEV]
GO
/****** Object:  DatabaseRole [FactoryRO]    Script Date: 30/05/2018 10:57:10 ******/
CREATE ROLE [FactoryRO]
GO
/****** Object:  StoredProcedure [dbo].[GetGamesVersionsAtDate]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetGamesVersionsAtDate] (@date datetime)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT        Game.MainGameType, Game.GameName, Game.IsExternal, Latest.Technology, GameVersion_1.VersionFolder, GameVersion_1.VersionAsLong, 
                         GameVersion_1.CreatedDate, GameVersion_1.CreatedBy, GameVersion_1.TriggeredBy, GameVersion_Regulation.Regulation, 
                         GameVersion_Regulation.DownloadUri
FROM            (SELECT        Game_ID, Technology, MAX(CreatedDate) AS LastDate
                          FROM            GameVersion
                          WHERE        (CreatedDate < @date)
                          GROUP BY Game_ID, Technology) AS Latest INNER JOIN
                         Game ON Latest.Game_ID = Game.Game_ID INNER JOIN
                         GameVersion AS GameVersion_1 ON Latest.Game_ID = GameVersion_1.Game_ID AND Latest.Technology = GameVersion_1.Technology AND 
                         Latest.LastDate = GameVersion_1.CreatedDate INNER JOIN
                         GameVersion_Regulation ON GameVersion_1.GameVersion_ID = GameVersion_Regulation.GameVersion_ID

order by MainGameType, Regulation
						 

END


GO
/****** Object:  StoredProcedure [dbo].[NormalizeApprovalStatusForAllLanguagesWithTheSameHash]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[ClientType]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientType](
	[ClientType_ID] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ClientType] PRIMARY KEY CLUSTERED 
(
	[ClientType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ClientTypeToRegulationMapping]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientTypeToRegulationMapping](
	[Row_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ClientType] [nvarchar](50) NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ClientTypeFromRegulationExclusion] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Unique_ClientTypeFromRegulationExclusion] UNIQUE NONCLUSTERED 
(
	[ClientType] ASC,
	[Regulation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Game]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Game](
	[Game_ID] [uniqueidentifier] NOT NULL,
	[GameName] [nvarchar](250) NOT NULL,
	[MainGameType] [int] NOT NULL,
	[IsExternal] [bit] NOT NULL,
	[ComponentCategory] [int] NOT NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[Game_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameTechnology]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameTechnology](
	[Technology_ID] [int] NOT NULL,
	[TechnologyName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_GameTechnology] PRIMARY KEY CLUSTERED 
(
	[Technology_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameType]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameType](
	[Row_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GameType_ID] [int] NOT NULL,
	[Game_ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Operator_ID] [int] NULL,
 CONSTRAINT [PK_Row_ID] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueGameTypePerGame] UNIQUE NONCLUSTERED 
(
	[GameType_ID] ASC,
	[Game_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion](
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[VersionFolder] [nvarchar](20) NOT NULL,
	[VersionAsLong] [bigint] NOT NULL,
	[Technology] [int] NOT NULL,
	[Game_ID] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[TriggeredBy] [nvarchar](50) NULL,
	[PlatformType] [int] NOT NULL,
 CONSTRAINT [PK_GameVersion] PRIMARY KEY CLUSTERED 
(
	[GameVersion_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueGameVersion] UNIQUE NONCLUSTERED 
(
	[Game_ID] ASC,
	[VersionAsLong] ASC,
	[VersionFolder] ASC,
	[Technology] ASC,
	[PlatformType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Language]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Language](
	[GameVersionLanguage_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[Language] [nvarchar](50) NOT NULL,
	[LanguageHash] [nvarchar](50) NOT NULL,
	[ArtifactoryLanguage] [nvarchar](50) NOT NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[ProductionUploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GameVersion_Language] PRIMARY KEY CLUSTERED 
(
	[GameVersionLanguage_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Language_ToArtifactorySyncQueue]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Language_ToArtifactorySyncQueue](
	[Row_ID] [int] IDENTITY(1,1) NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[InsertedTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_GameVersion_LanguageHashQueue] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Property]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Property](
	[Row_ID] [uniqueidentifier] NOT NULL,
	[PropertyKey] [nvarchar](50) NOT NULL,
	[PropertyValue] [nvarchar](max) NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[LastChange] [datetime2](7) NULL,
	[ChangedBy] [nvarchar](50) NULL,
	[IsUserAdded] [bit] NOT NULL,
	[PropertyName] [nvarchar](50) NULL,
	[PropertySet] [nvarchar](50) NULL,
 CONSTRAINT [PK_GameVersion_Property] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Property_History]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Property_History](
	[Row_ID] [int] IDENTITY(1,1) NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[PropertyKey] [nvarchar](50) NOT NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[ChangeDate] [datetime2](7) NOT NULL,
	[ChangedBy] [nvarchar](50) NOT NULL,
	[ChangeType] [int] NOT NULL,
 CONSTRAINT [PK_GameVersion_Property_History] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Regulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Regulation](
	[GameVersionRegulation_ID] [uniqueidentifier] NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[DownloadUri] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](260) NOT NULL,
	[FileSize] [bigint] NULL,
	[MD5] [nvarchar](50) NULL,
	[SHA1] [nvarchar](50) NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[PMApprovalDate] [datetime2](7) NULL,
	[PMApprovalUser] [nvarchar](50) NULL,
	[ProductionUploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GameVersion_DownloadUri] PRIMARY KEY CLUSTERED 
(
	[GameVersionRegulation_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_GameVersion_DownloadUri_UniqueDownloadUri] UNIQUE NONCLUSTERED 
(
	[GameVersion_ID] ASC,
	[Regulation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameVersion_Regulation_ClientType]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameVersion_Regulation_ClientType](
	[GameVersionRegulationClient_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GameVersionRegulation_ID] [uniqueidentifier] NOT NULL,
	[ClientType_ID] [nvarchar](20) NOT NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[PMApprovalDate] [datetime2](7) NULL,
	[PMApprovalUser] [nvarchar](50) NULL,
	[ProductionUploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GameVersion_Regulation_ClientType] PRIMARY KEY CLUSTERED 
(
	[GameVersionRegulationClient_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueClientTypePerGameVersionAndRegulation] UNIQUE NONCLUSTERED 
(
	[GameVersionRegulationClient_ID] ASC,
	[ClientType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlatformType]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlatformType](
	[PlatformType_ID] [int] NOT NULL,
	[PlatformName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PlatformType] PRIMARY KEY CLUSTERED 
(
	[PlatformType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegulationType]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegulationType](
	[RegulationType_ID] [int] NOT NULL,
	[RegulationName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RegulationType] PRIMARY KEY CLUSTERED 
(
	[RegulationType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegulationType_MandatoryLanguage]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegulationType_MandatoryLanguage](
	[RegulationTypeLanguage_ID] [int] IDENTITY(1,1) NOT NULL,
	[RegulationType_ID] [int] NOT NULL,
	[LanguageIso3] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_RegulationType_MandatoryLanguage] PRIMARY KEY CLUSTERED 
(
	[RegulationTypeLanguage_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_RegulationType_MandatoryLanguage_UniqueMandatoryLanguagePerRegulation] UNIQUE NONCLUSTERED 
(
	[RegulationTypeLanguage_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UpgradeScripts]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UpgradeScripts](
	[Script_ID] [int] IDENTITY(1,1) NOT NULL,
	[ScriptName] [nvarchar](50) NOT NULL,
	[RunDateTime] [datetime] NOT NULL,
	[ScriptContent] [ntext] NULL,
 CONSTRAINT [PK_UpgradeScripts] PRIMARY KEY CLUSTERED 
(
	[Script_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  View [dbo].[LastVersionForEachGame]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[LastVersionForEachGame]
AS
SELECT        Game_ID, Technology, PlatformType, MAX(VersionAsLong) AS LatestVersion
FROM            dbo.GameVersion
GROUP BY Game_ID, Technology, PlatformType


GO
/****** Object:  View [dbo].[__latestGameVersionWithUnapprovedLanguages]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[__latestGameVersionWithUnapprovedLanguages]
AS
SELECT        TOP (100) PERCENT dbo.Game.MainGameType AS GameType, dbo.Game.GameName, dbo.GameTechnology.TechnologyName AS Technology, 
                         dbo.PlatformType.PlatformName AS Platform, dbo.GameVersion.VersionFolder AS Version, dbo.GameVersion_Language.Regulation, 
                         dbo.GameVersion_Language.Language, dbo.GameVersion.VersionAsLong
FROM            dbo.GameVersion_Language INNER JOIN
                         dbo.GameVersion ON dbo.GameVersion_Language.GameVersion_ID = dbo.GameVersion.GameVersion_ID INNER JOIN
                         dbo.LastVersionForEachGame INNER JOIN
                         dbo.Game ON dbo.LastVersionForEachGame.Game_ID = dbo.Game.Game_ID ON dbo.GameVersion.Game_ID = dbo.LastVersionForEachGame.Game_ID AND 
                         dbo.GameVersion.Technology = dbo.LastVersionForEachGame.Technology AND dbo.GameVersion.PlatformType = dbo.LastVersionForEachGame.PlatformType AND 
                         dbo.GameVersion.VersionAsLong = dbo.LastVersionForEachGame.LatestVersion INNER JOIN
                         dbo.GameTechnology ON dbo.LastVersionForEachGame.Technology = dbo.GameTechnology.Technology_ID INNER JOIN
                         dbo.PlatformType ON dbo.LastVersionForEachGame.PlatformType = dbo.PlatformType.PlatformType_ID
WHERE        (dbo.GameVersion_Language.QAApprovalDate IS NULL)

GO
/****** Object:  View [dbo].[GameInfrastructure]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[GameInfrastructure]
AS
SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation
FROM            dbo.GameVersion INNER JOIN
                         dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation


GO
/****** Object:  View [dbo].[LastQAApprovedVersionForEachGame]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[LastQAApprovedVersionForEachGame]
AS
SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, MAX(dbo.GameVersion.VersionAsLong) 
                         AS LatestQAApprovedVersion
FROM            dbo.GameVersion INNER JOIN
                         dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
WHERE        (dbo.GameVersion_Regulation.QAApprovalDate IS NOT NULL)
GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType


GO
/****** Object:  View [dbo].[LatestPMApprovedGameVersionForEachRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[LatestPMApprovedGameVersionForEachRegulation]
AS
SELECT        PMLatestApproved.Game_ID, PMLatestApproved.LastVersion, PMLatestApproved.Technology, PMLatestApproved.PlatformType, PMLatestApproved.Regulation, 
                         GameVersion_1.GameVersion_ID, GameVersion_Regulation_1.DownloadUri, GameVersion_Regulation_1.FileName, GameVersion_Regulation_1.FileSize, 
                         GameVersion_Regulation_1.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
                          WHERE        (dbo.GameVersion_Regulation.PMApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation) 
                         AS PMLatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON PMLatestApproved.Game_ID = GameVersion_1.Game_ID AND 
                         PMLatestApproved.LastVersion = GameVersion_1.VersionAsLong AND PMLatestApproved.Technology = GameVersion_1.Technology AND 
                         PMLatestApproved.PlatformType = GameVersion_1.PlatformType INNER JOIN
                         dbo.GameVersion_Regulation AS GameVersion_Regulation_1 ON GameVersion_1.GameVersion_ID = GameVersion_Regulation_1.GameVersion_ID AND 
                         PMLatestApproved.Regulation = GameVersion_Regulation_1.Regulation


GO
/****** Object:  View [dbo].[LatestProductionUploadGameVersionForEachRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
/****** Object:  View [dbo].[LatestQAApprovedGameVersionForEachRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[LatestQAApprovedGameVersionForEachRegulation]
AS
SELECT        QALatestApproved.Game_ID, QALatestApproved.LastVersion, QALatestApproved.Technology, QALatestApproved.PlatformType, QALatestApproved.Regulation, 
                         GameVersion_1.GameVersion_ID, GameVersion_Regulation_1.DownloadUri, GameVersion_Regulation_1.FileName, GameVersion_Regulation_1.FileSize, 
                         GameVersion_Regulation_1.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, MAX(dbo.GameVersion.VersionAsLong) AS LastVersion, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, 
                                                    dbo.GameVersion_Regulation.Regulation
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Regulation ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID
                          WHERE        (dbo.GameVersion_Regulation.QAApprovalDate IS NOT NULL)
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Regulation.Regulation) 
                         AS QALatestApproved INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON QALatestApproved.LastVersion = GameVersion_1.VersionAsLong AND 
                         QALatestApproved.Technology = GameVersion_1.Technology AND QALatestApproved.PlatformType = GameVersion_1.PlatformType AND 
                         QALatestApproved.Game_ID = GameVersion_1.Game_ID INNER JOIN
                         dbo.GameVersion_Regulation AS GameVersion_Regulation_1 ON GameVersion_1.GameVersion_ID = GameVersion_Regulation_1.GameVersion_ID AND 
                         QALatestApproved.Regulation = GameVersion_Regulation_1.Regulation


GO
/****** Object:  View [dbo].[LatestApprovedGameVersionForEachRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[LatestApprovedGameVersionForEachRegulation]
AS
SELECT DISTINCT 
                         dbo.GameInfrastructure.Game_ID, dbo.Game.GameName, dbo.Game.MainGameType, dbo.LastVersionForEachGame.LatestVersion, dbo.Game.IsExternal, 
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
/****** Object:  View [dbo].[NeverApprovedGames]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[NeverApprovedGames]
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
/****** Object:  View [dbo].[GameVersion_LanguageQAApprovalInfo]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GameVersion_LanguageQAApprovalInfo]
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
/****** Object:  View [dbo].[GameVersion_SupportedRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GameVersion_SupportedRegulation]
AS
SELECT DISTINCT GameVersion_ID, Regulation
FROM            dbo.GameVersion_Property

GO
/****** Object:  View [dbo].[LatestVersionForEachGameAndRegulation]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[LatestVersionForEachGameAndRegulation]
AS
SELECT        GameVersion_1.GameVersion_ID, Latest.Game_ID, Latest.Technology, Latest.PlatformType, Latest.Regulation, Latest.LastVersion AS VersionAsLong, 
                         GameVersion_1.VersionFolder, dbo.Game.GameName, dbo.Game.MainGameType, dbo.Game.IsExternal, dbo.GameVersion_Regulation.DownloadUri, 
                         dbo.GameVersion_Regulation.FileName, dbo.GameVersion_Regulation.FileSize, dbo.GameVersion_Regulation.MD5
FROM            (SELECT        dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Property.Regulation, 
                                                    MAX(dbo.GameVersion.VersionAsLong) AS LastVersion
                          FROM            dbo.GameVersion INNER JOIN
                                                    dbo.GameVersion_Property ON dbo.GameVersion.GameVersion_ID = dbo.GameVersion_Property.GameVersion_ID
                          GROUP BY dbo.GameVersion.Game_ID, dbo.GameVersion.Technology, dbo.GameVersion.PlatformType, dbo.GameVersion_Property.Regulation) 
                         AS Latest INNER JOIN
                         dbo.GameVersion AS GameVersion_1 ON Latest.Game_ID = GameVersion_1.Game_ID AND Latest.Technology = GameVersion_1.Technology AND 
                         Latest.LastVersion = GameVersion_1.VersionAsLong AND Latest.PlatformType = GameVersion_1.PlatformType INNER JOIN
                         dbo.Game ON GameVersion_1.Game_ID = dbo.Game.Game_ID INNER JOIN
                         dbo.GameVersion_Regulation ON GameVersion_1.GameVersion_ID = dbo.GameVersion_Regulation.GameVersion_ID AND 
                         Latest.Regulation = dbo.GameVersion_Regulation.Regulation


GO
/****** Object:  View [dbo].[RegulationsNames]    Script Date: 30/05/2018 10:57:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RegulationsNames]
AS
SELECT DISTINCT Regulation as Name
FROM            dbo.GameVersion_Property


GO
ALTER TABLE [dbo].[ClientTypeToRegulationMapping] ADD  CONSTRAINT [DF_ClientTypeFromRegulationExclusion_Row_ID]  DEFAULT (newid()) FOR [Row_ID]
GO
ALTER TABLE [dbo].[GameType] ADD  CONSTRAINT [DF_GameType_Row_ID]  DEFAULT (newid()) FOR [Row_ID]
GO
ALTER TABLE [dbo].[GameVersion_Language] ADD  CONSTRAINT [DF_GameVersion_Language_GameVersionLanguage_ID]  DEFAULT (newid()) FOR [GameVersionLanguage_ID]
GO
ALTER TABLE [dbo].[GameVersion_Language_ToArtifactorySyncQueue] ADD  CONSTRAINT [DF_GameVersion_LanguageHashQueue_InsertedTime]  DEFAULT (getdate()) FOR [InsertedTime]
GO
ALTER TABLE [dbo].[GameVersion_Regulation_ClientType] ADD  CONSTRAINT [DF_GameVersion_Regulation_ClientType_GameVersionRegulationClient_ID]  DEFAULT (newid()) FOR [GameVersionRegulationClient_ID]
GO
ALTER TABLE [dbo].[GameType]  WITH CHECK ADD  CONSTRAINT [FK_GameType_Game] FOREIGN KEY([Game_ID])
REFERENCES [dbo].[Game] ([Game_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameType] CHECK CONSTRAINT [FK_GameType_Game]
GO
ALTER TABLE [dbo].[GameVersion]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Game] FOREIGN KEY([Game_ID])
REFERENCES [dbo].[Game] ([Game_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion] CHECK CONSTRAINT [FK_GameVersion_Game]
GO
ALTER TABLE [dbo].[GameVersion_Language]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Language_GameVersion_Language] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion_Language] CHECK CONSTRAINT [FK_GameVersion_Language_GameVersion_Language]
GO
ALTER TABLE [dbo].[GameVersion_Property]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Property_GameVersion] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion_Property] CHECK CONSTRAINT [FK_GameVersion_Property_GameVersion]
GO
ALTER TABLE [dbo].[GameVersion_Property_History]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Property_History_GameVersion] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion_Property_History] CHECK CONSTRAINT [FK_GameVersion_Property_History_GameVersion]
GO
ALTER TABLE [dbo].[GameVersion_Regulation]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_DownloadUri_GameVersion] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion_Regulation] CHECK CONSTRAINT [FK_GameVersion_DownloadUri_GameVersion]
GO
ALTER TABLE [dbo].[GameVersion_Regulation_ClientType]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Regulation_ClientType_GameVersion_Regulation] FOREIGN KEY([GameVersionRegulation_ID])
REFERENCES [dbo].[GameVersion_Regulation] ([GameVersionRegulation_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GameVersion_Regulation_ClientType] CHECK CONSTRAINT [FK_GameVersion_Regulation_ClientType_GameVersion_Regulation]
GO
ALTER TABLE [dbo].[RegulationType_MandatoryLanguage]  WITH CHECK ADD  CONSTRAINT [FK_RegulationType_MandatoryLanguage_RegulationType] FOREIGN KEY([RegulationType_ID])
REFERENCES [dbo].[RegulationType] ([RegulationType_ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RegulationType_MandatoryLanguage] CHECK CONSTRAINT [FK_RegulationType_MandatoryLanguage_RegulationType]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "GameVersion_Language"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 265
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "GameVersion"
            Begin Extent = 
               Top = 6
               Left = 303
               Bottom = 135
               Right = 478
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LastVersionForEachGame"
            Begin Extent = 
               Top = 6
               Left = 516
               Bottom = 159
               Right = 686
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Game"
            Begin Extent = 
               Top = 6
               Left = 724
               Bottom = 135
               Right = 925
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "GameTechnology"
            Begin Extent = 
               Top = 6
               Left = 963
               Bottom = 101
               Right = 1147
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PlatformType"
            Begin Extent = 
               Top = 6
               Left = 1185
               Bottom = 101
               Right = 1362
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'__latestGameVersionWithUnapprovedLanguages'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'        Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'__latestGameVersionWithUnapprovedLanguages'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'__latestGameVersionWithUnapprovedLanguages'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[7] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "GameVersion_Property"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 236
               Right = 347
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 3825
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GameVersion_SupportedRegulation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GameVersion_SupportedRegulation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "GameInfrastructure"
            Begin Extent = 
               Top = 4
               Left = 12
               Bottom = 150
               Right = 182
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LastVersionForEachGame"
            Begin Extent = 
               Top = 0
               Left = 758
               Bottom = 129
               Right = 928
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Game"
            Begin Extent = 
               Top = 120
               Left = 872
               Bottom = 249
               Right = 1073
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RegulationType"
            Begin Extent = 
               Top = 116
               Left = 748
               Bottom = 211
               Right = 936
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LastQAApprovedVersionForEachGame"
            Begin Extent = 
               Top = 227
               Left = 735
               Bottom = 356
               Right = 963
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LatestProductionUploadGameVersionForEachRegulation"
            Begin Extent = 
               Top = 245
               Left = 416
               Bottom = 374
               Right = 712
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LatestQAApprovedGameVersionForEachRegulation"
            Begin Extent = 
               Top =' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LatestApprovedGameVersionForEachRegulation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' 402
               Left = 38
               Bottom = 531
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LatestPMApprovedGameVersionForEachRegulation"
            Begin Extent = 
               Top = 402
               Left = 251
               Bottom = 531
               Right = 426
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LatestApprovedGameVersionForEachRegulation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LatestApprovedGameVersionForEachRegulation'
GO
