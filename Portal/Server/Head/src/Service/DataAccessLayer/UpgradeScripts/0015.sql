CREATE TABLE [dbo].[GameVersion_LanguageQueue](
	[Row_ID] [int] IDENTITY(1,1) NOT NULL,
	[GameVersion_ID] [uniqueidentifier] NOT NULL,
	[InsertedTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_GameVersion_LanguageQueue] PRIMARY KEY CLUSTERED 
(
	[Row_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GameVersion_LanguageQueue] ADD  CONSTRAINT [DF_GameVersion_LanguageQueue_InsertedTime]  DEFAULT (getdate()) FOR [InsertedTime]
GO


