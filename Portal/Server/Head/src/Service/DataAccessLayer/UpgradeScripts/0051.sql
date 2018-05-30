ALTER TABLE dbo.GameVersion_Regulation ADD
	QAApprovalDate datetime2(7) NULL,
	QAApprovalUser nvarchar(50) NULL,
	PMApprovalDate datetime2(7) NULL,
	PMApprovalUser nvarchar(50) NULL,
	ProductionUploadDate datetime2(7) NULL
GO

