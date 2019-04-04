CREATE TABLE [dbo].[GamingComponentType](
	[ComponentType_ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_GamingComponentType] PRIMARY KEY CLUSTERED 
(
	[ComponentType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[GamingComponent](
	[GamingComponent_ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ComponentType] [int] NOT NULL,
	[Technology] [int] NOT NULL,
	[RepositoryName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_GamingComponent_1] PRIMARY KEY CLUSTERED 
(
	[GamingComponent_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponent]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponent_GamingComponentType] FOREIGN KEY([ComponentType])
REFERENCES [dbo].[GamingComponentType] ([ComponentType_ID])
GO

ALTER TABLE [dbo].[GamingComponent] CHECK CONSTRAINT [FK_GamingComponent_GamingComponentType]
GO

CREATE TABLE [dbo].[GamingComponentVersion](
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[GamingComponent_ID] [uniqueidentifier] NOT NULL,
	[VersionFolder] [nvarchar](20) NOT NULL,
	[VersionAsLong] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[TriggeredBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_GamingComponentVersion] PRIMARY KEY CLUSTERED 
(
	[GamingComponentVersion_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_GamingComponentVersion] FOREIGN KEY([GamingComponent_ID])
REFERENCES [dbo].[GamingComponent] ([GamingComponent_ID])
GO

ALTER TABLE [dbo].[GamingComponentVersion] CHECK CONSTRAINT [FK_GamingComponentVersion_GamingComponentVersion]
GO

CREATE TABLE [dbo].[GamingComponentVersion_DownloadUri](
	[Row_ID] [uniqueidentifier] NOT NULL,
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[DownloadUri] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](260) NOT NULL,
	[FileSize] [bigint] NULL,
	[MD5] [nvarchar](50) NULL,
	[SHA1] [nvarchar](50) NULL,
 CONSTRAINT [PK_GamingComponentVersion_DownloadUri] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_DownloadUri]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_DownloadUri_GamingComponentVersion1] FOREIGN KEY([GamingComponentVersion_ID])
REFERENCES [dbo].[GamingComponentVersion] ([GamingComponentVersion_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GamingComponentVersion_DownloadUri] CHECK CONSTRAINT [FK_GamingComponentVersion_DownloadUri_GamingComponentVersion1]
GO


CREATE TABLE [dbo].[GamingComponentVersion_Property](
	[Row_ID] [uniqueidentifier] NOT NULL,
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[PropertyKey] [nvarchar](50) NOT NULL,
	[PropertyValue] [nvarchar](max) NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[LastChange] [datetime2](7) NULL,
	[ChangedBy] [nvarchar](50) NULL,
	[IsUserAdded] [bit] NOT NULL,
	[PropertyName] [nvarchar](50) NULL,
	[PropertySet] [nvarchar](50) NULL,
 CONSTRAINT [PK_GamingComponentVersion_Property] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_Property]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_Property_GamingComponentVersion1] FOREIGN KEY([GamingComponentVersion_ID])
REFERENCES [dbo].[GamingComponentVersion] ([GamingComponentVersion_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GamingComponentVersion_Property] CHECK CONSTRAINT [FK_GamingComponentVersion_Property_GamingComponentVersion1]
GO

CREATE TABLE [dbo].[GamingComponentVersion_Property_History](
	[Row_ID] [int] IDENTITY(1,1) NOT NULL,
	[GamingComponentVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[PropertyKey] [nvarchar](50) NOT NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[ChangeDate] [datetime2](7) NOT NULL,
	[ChangedBy] [nvarchar](50) NOT NULL,
	[ChangeType] [int] NOT NULL,
 CONSTRAINT [PK_GamingComponentVersion_Property_History] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[GamingComponentVersion_Property_History]  WITH CHECK ADD  CONSTRAINT [FK_GamingComponentVersion_Property_History_GamingComponentVersion1] FOREIGN KEY([GamingComponentVersion_ID])
REFERENCES [dbo].[GamingComponentVersion] ([GamingComponentVersion_ID])
GO

ALTER TABLE [dbo].[GamingComponentVersion_Property_History] CHECK CONSTRAINT [FK_GamingComponentVersion_Property_History_GamingComponentVersion1]
GO




INSERT INTO [dbo].[GamingComponentType] ([ComponentType_ID] ,[Name]) VALUES  (0, 'Wrapper')

GO

INSERT INTO [dbo].[GamingComponentType] ([ComponentType_ID] ,[Name]) VALUES  (1, 'Chill')
GO


INSERT INTO [dbo].[GamingComponent] ([GamingComponent_ID] ,[Name] ,[ComponentType] ,[Technology] ,[RepositoryName]) VALUES ('04831D5B-79C8-4017-BF36-470774A195A9' ,'Wrapper',0 ,0 ,'modernGame-local')
GO

INSERT INTO [dbo].[GamingComponent] ([GamingComponent_ID] ,[Name] ,[ComponentType] ,[Technology] ,[RepositoryName]) VALUES ('597052BF-01C0-49EB-A501-464406A90C07' ,'Chill',1 ,1 ,'HTML5Game-local')
GO





