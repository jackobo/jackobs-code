ALTER TABLE dbo.GameVersion_DownloadUri ADD
	FileSize bigint NULL,
	MD5 nvarchar(50) NULL,
	SHA1 nvarchar(50)  NULL
GO
