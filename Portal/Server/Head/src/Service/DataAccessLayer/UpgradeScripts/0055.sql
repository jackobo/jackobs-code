Update [dbo].[GameVersion] set [Technology] = 0, [PlatformType] = 1 From GameVersion
where Game_ID in (Select Game_ID From Game where ComponentCategory = 0)