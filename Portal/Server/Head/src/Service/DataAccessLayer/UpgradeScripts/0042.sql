ALTER TABLE dbo.GamingComponentVersion ADD
	Technology int NULL
GO

Update dbo.GamingComponentVersion Set Technology = [dbo].[GamingComponent].[Technology]
From dbo.GamingComponentVersion Inner Join [dbo].[GamingComponent] 
on dbo.GamingComponentVersion.GamingComponent_ID = [dbo].[GamingComponent] .GamingComponent_ID

GO

ALTER TABLE dbo.GamingComponentVersion alter column  Technology int not NULL
GO

DROP TABLE [dbo].[GameVersion_Language_FromArtifactorySyncQueue]
GO


