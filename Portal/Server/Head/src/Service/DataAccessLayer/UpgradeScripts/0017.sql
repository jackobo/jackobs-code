alter table [dbo].[GameVersion_Language] alter column Language nvarchar(50) not null
go
alter table [dbo].[GameVersion_Language] add ArtifactoryLanguage nvarchar(50) not null