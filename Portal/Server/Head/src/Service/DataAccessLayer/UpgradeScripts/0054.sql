Update [dbo].[GameVersion_Regulation] Set QAApprovalDate  = LastQAApproval.LastQAApprovalDate, QAApprovalUser = LastQAApproval.LastQAApprovalUser
From [dbo].[GameVersion_Regulation]
inner join 
(SELECT GameVersionRegulation_ID, Max(QAApprovalDate) as LastQAApprovalDate, Max(QAApprovalUser)  as LastQAApprovalUser
FROM [dbo].[GameVersion_Regulation_ClientType] WHERE QAApprovalDate IS NOT NULL
group by GameVersionRegulation_ID) LastQAApproval
on [dbo].[GameVersion_Regulation].GameVersionRegulation_ID = LastQAApproval.GameVersionRegulation_ID

GO

Update [dbo].[GameVersion_Regulation] Set PMApprovalDate  = LastPMApproval.LastPMApprovalDate, PMApprovalUser = LastPMApproval.LastPMApprovalUser
From [dbo].[GameVersion_Regulation]
inner join 
(SELECT GameVersionRegulation_ID, Max(PMApprovalDate) as LastPMApprovalDate, Max(PMApprovalUser)  as LastPMApprovalUser
FROM [dbo].[GameVersion_Regulation_ClientType] WHERE PMApprovalDate IS NOT NULL
group by GameVersionRegulation_ID) LastPMApproval
on [dbo].[GameVersion_Regulation].GameVersionRegulation_ID = LastPMApproval.GameVersionRegulation_ID

GO

Update [dbo].[GameVersion_Regulation] Set ProductionUploadDate  = LastUpload.LastUploadDate
From [dbo].[GameVersion_Regulation]
inner join 
(SELECT GameVersionRegulation_ID, Max(ProductionUploadDate) as LastUploadDate
FROM [dbo].[GameVersion_Regulation_ClientType] WHERE ProductionUploadDate IS NOT NULL
group by GameVersionRegulation_ID) LastUpload
on [dbo].[GameVersion_Regulation].GameVersionRegulation_ID = LastUpload.GameVersionRegulation_ID

