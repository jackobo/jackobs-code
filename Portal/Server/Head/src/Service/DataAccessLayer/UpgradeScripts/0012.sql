ALTER TABLE dbo.GameVersion ADD PlatformType int NULL
GO
ALTER TABLE dbo.GamingComponent ADD
	PlatformType int NULL
GO
update [dbo].[GamingComponent] set PlatformType = 3 where [Name] = 'Chill'
GO
update [dbo].[GamingComponent] set PlatformType = 1 where [Name] = 'Wrapper'
GO
alter table [dbo].[GamingComponent] alter column PlatformType int not null
GO
ALTER TABLE dbo.GamingComponentVersion ADD
	PlatformType int NULL
GO