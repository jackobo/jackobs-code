CREATE TABLE [dbo].[GamingComponentVersion_Language](
	[GamingComponentVersionLanguage_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[Language] [nvarchar](50) NOT NULL,
	[LanguageHash] [nvarchar](50) NOT NULL,
	[ArtifactoryLanguage] [nvarchar](50) NOT NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[ProductionUploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GamingComponentVersion_Language] PRIMARY KEY CLUSTERED 
(
	[GamingComponentVersionLanguage_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_Language] ADD  CONSTRAINT [DF_GamingComponentVersion_Language_GamingComponentVersionLanguage_ID]  DEFAULT (newid()) FOR [GamingComponentVersionLanguage_ID]
GO

ALTER TABLE [dbo].[GamingComponentVersion_Language]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_Language_GamingComponentVersion] FOREIGN KEY([GamingComponentVersion_ID])
REFERENCES [dbo].[GamingComponentVersion] ([GamingComponentVersion_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GamingComponentVersion_Language] CHECK CONSTRAINT [FK_GamingComponentVersion_Language_GamingComponentVersion]
GO


CREATE TABLE [dbo].[GamingComponentVersion_Regulation_ClientType](
	[GamingComponentVersionRegulationClientType_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GamingComponentVersionRegulation_ID] [uniqueidentifier] NOT NULL,
	[ClientType_ID] [nvarchar](20) NOT NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[PMApprovalDate] [datetime2](7) NULL,
	[PMApprovalUser] [nvarchar](50) NULL,
	[ProductionUploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GamingComponentVersion_Regulation_ClientType] PRIMARY KEY CLUSTERED 
(
	[GamingComponentVersionRegulationClientType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueClientTypePerGamingComponentVersionAndRegulation] UNIQUE NONCLUSTERED 
(
	[GamingComponentVersionRegulation_ID] ASC,
	[ClientType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_Regulation_ClientType]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_Regulation_ClientType_GamingComponentVersion_Regulation] FOREIGN KEY([GamingComponentVersionRegulation_ID])
REFERENCES [dbo].[GamingComponentVersion_Regulation] ([GamingComponentVersionRegulation_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GamingComponentVersion_Regulation_ClientType] CHECK CONSTRAINT [FK_GamingComponentVersion_Regulation_ClientType_GamingComponentVersion_Regulation]
GO



CREATE TABLE [dbo].[GamingComponentVersion_Language_ToArtifactorySyncQueue](
	[Row_ID] [int] NOT NULL,
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[InsertedTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_GamingComponentVersion_Language_ToArtifactorySyncQueue] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_Language_ToArtifactorySyncQueue] ADD  CONSTRAINT [DF_GamingComponentVersion_Language_ToArtifactorySyncQueue_InsertedTime]  DEFAULT (getdate()) FOR [InsertedTime]
GO

CREATE VIEW [dbo].[ChillWrapper_LanguageQAApprovalInfo]
AS
SELECT DISTINCT 
                         dbo.GamingComponentVersion_Language.GamingComponentVersionLanguage_ID, ApprovedLanguage.QAApprovalDate, ApprovedLanguage.QAApprovalUser
FROM            dbo.GamingComponentVersion_Language INNER JOIN
                             (SELECT        LanguageHash, ArtifactoryLanguage, MAX(QAApprovalDate) AS QAApprovalDate, MAX(QAApprovalUser) AS QAApprovalUser
                               FROM            dbo.GamingComponentVersion_Language AS GamingComponentVersion_Language_1
                               WHERE        (QAApprovalDate IS NOT NULL)
                               GROUP BY ArtifactoryLanguage, LanguageHash) AS ApprovedLanguage ON 
                         dbo.GamingComponentVersion_Language.ArtifactoryLanguage = ApprovedLanguage.ArtifactoryLanguage AND 
                         dbo.GamingComponentVersion_Language.LanguageHash = ApprovedLanguage.LanguageHash

GO


