CREATE TABLE [dbo].[GameVersion_DownloadUri](
	[Row_ID] [uniqueidentifier] NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[DownloadUri] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_GameVersion_DownloadUri] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_GameVersion_DownloadUri_UniqueDownloadUri] UNIQUE NONCLUSTERED 
(
	[GameVersion_ID] ASC,
	[Regulation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Index [IX_GameVersion_DownloadUri_GameVersion_ID]    Script Date: 01/04/2016 09:57:03 ******/
CREATE NONCLUSTERED INDEX [IX_GameVersion_DownloadUri_GameVersion_ID] ON [dbo].[GameVersion_DownloadUri]
(
	[GameVersion_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_GameVersion_DownloadUri_Regulation]    Script Date: 01/04/2016 09:57:03 ******/
CREATE NONCLUSTERED INDEX [IX_GameVersion_DownloadUri_Regulation] ON [dbo].[GameVersion_DownloadUri]
(
	[Regulation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GameVersion_DownloadUri]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_DownloadUri_GameVersion] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GameVersion_DownloadUri] CHECK CONSTRAINT [FK_GameVersion_DownloadUri_GameVersion]
GO

alter table [dbo].[GameVersion_DownloadUri] add FileName nvarchar(260) not null
GO

