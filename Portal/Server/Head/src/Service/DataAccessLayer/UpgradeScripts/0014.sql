CREATE TABLE [dbo].[GameVersion_Language](
	[GameVersionLanguage_ID] [uniqueidentifier] NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[Regulation] [nvarchar](50) NOT NULL,
	[Language] [nvarchar](5) NOT NULL,
	[Hash] [nvarchar](50) NOT NULL,
	[IsQAApproved] [bit] NOT NULL,
	[IsInProduction] [bit] NOT NULL,
 CONSTRAINT [PK_GameVersion_Language] PRIMARY KEY CLUSTERED 
(
	[GameVersionLanguage_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GameVersion_Language] ADD  CONSTRAINT [DF_GameVersion_Language_IsApproved]  DEFAULT ((0)) FOR [IsQAApproved]
GO

ALTER TABLE [dbo].[GameVersion_Language] ADD  CONSTRAINT [DF_GameVersion_Language_IsInProduction]  DEFAULT ((0)) FOR [IsInProduction]
GO

ALTER TABLE [dbo].[GameVersion_Language]  WITH CHECK ADD  CONSTRAINT [FK_GameVersion_Language_GameVersion_Language] FOREIGN KEY([GameVersion_ID])
REFERENCES [dbo].[GameVersion] ([GameVersion_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[GameVersion_Language] CHECK CONSTRAINT [FK_GameVersion_Language_GameVersion_Language]
GO

