CREATE TABLE [dbo].[GameVersion_LanguageHashQueue](
	[Row_ID] [int] IDENTITY(1,1) NOT NULL,
	[LanguageHash] [nvarchar](50) NOT NULL,
	[InsertedTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_GameVersion_LanguageHashQueue] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GameVersion_LanguageHashQueue] ADD  CONSTRAINT [DF_GameVersion_LanguageHashQueue_InsertedTime]  DEFAULT (getdate()) FOR [InsertedTime]
GO


