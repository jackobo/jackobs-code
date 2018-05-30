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

ALTER TABLE [dbo].[RegulationType_MandatoryLanguage]  WITH CHECK ADD  CONSTRAINT [FK_RegulationType_MandatoryLanguage_RegulationType] FOREIGN KEY([RegulationType_ID])
REFERENCES [dbo].[RegulationType] ([RegulationType_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RegulationType_MandatoryLanguage] CHECK CONSTRAINT [FK_RegulationType_MandatoryLanguage_RegulationType]
GO
