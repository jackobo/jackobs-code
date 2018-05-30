CREATE TABLE [dbo].[GameVersion_Regulation_ClientType](
	[GameVersionRegulationClient_ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GameVersionRegulation_ID] [uniqueidentifier] NOT NULL,
	[ClientType_ID] [nvarchar](10) NOT NULL,
	[QAApprovalDate] [datetime2](7) NULL,
	[QAApprovalUser] [nvarchar](50) NULL,
	[PMApprovalDate] [datetime2](7) NULL,
	[PMApprovalUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_GameVersion_Regulation_ClientType] PRIMARY KEY CLUSTERED 
(
	[GameVersionRegulationClient_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueClientTypePerGameVersionAndRegulation] UNIQUE NONCLUSTERED 
(
	[GameVersionRegulationClient_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GameVersion_Regulation_ClientType] ADD  CONSTRAINT [DF_GameVersion_Regulation_ClientType_GameVersionRegulationClient_ID]  DEFAULT (newid()) FOR [GameVersionRegulationClient_ID]
GO

ALTER TABLE [dbo].[GameVersion_Regulation_ClientType]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Regulation_ClientType_GameVersion_Regulation] FOREIGN KEY([GameVersionRegulation_ID])
REFERENCES [dbo].[GameVersion_Regulation] ([GameVersionRegulation_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GameVersion_Regulation_ClientType] CHECK CONSTRAINT [FK_GameVersion_Regulation_ClientType_GameVersion_Regulation]
GO

alter table [dbo].[GameVersion_Regulation_ClientType] alter column [ClientType_ID] nvarchar(20) NOT NULL

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

CREATE NONCLUSTERED INDEX IX_GameVersion_Regulation_ClientType_GameVersionRegulation_ID ON dbo.GameVersion_Regulation_ClientType
	(
	GameVersionRegulation_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_GameVersion_Regulation_ClientType_ClientType_ID ON dbo.GameVersion_Regulation_ClientType
	(
	ClientType_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.GameVersion_Regulation_ClientType SET (LOCK_ESCALATION = TABLE)
GO

INSERT INTO [dbo].[ClientType] ([ClientType_ID] ,[Description])
SELECT distinct [ClientType], [ClientType] FROM [dbo].[ClientTypeToRegulationMapping]
GO

INSERT INTO [dbo].[GameVersion_Regulation_ClientType]
           ([GameVersionRegulation_ID]
           ,[ClientType_ID])

SELECT    distinct    GVR.GameVersionRegulation_ID, GVP.PropertySet
FROM            GameVersion_Property AS GVP INNER JOIN
                         ClientType AS CT ON GVP.PropertySet = CT.ClientType_ID INNER JOIN
                         GameVersion_Regulation AS GVR ON GVP.GameVersion_ID = GVR.GameVersion_ID AND GVP.Regulation = GVR.Regulation
GO


