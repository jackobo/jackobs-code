ALTER TABLE dbo.Game ADD ComponentType int NULL
GO
UPDATE dbo.Game Set ComponentType = 2
UPDATE dbo.Game Set ComponentType = MainGameType where MainGameType < 2

ALTER TABLE dbo.Game ALTER COLUMN ComponentType int NOT NULL
