alter table [dbo].[GameVersion] alter column [CreatedDate] DateTime2(7) not null
GO
alter table [dbo].[GameVersion_Property] alter column [LastChange] DateTime2(7) null
GO
alter table [dbo].[GameVersion_Property_History] alter column [ChangeDate] DateTime2(7) not null