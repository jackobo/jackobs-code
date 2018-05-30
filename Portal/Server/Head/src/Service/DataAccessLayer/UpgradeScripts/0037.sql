ALTER TABLE dbo.GameVersion_Regulation_ClientType ADD
	ProductionUploadDate datetime2(7) NULL
GO

Update    GameVersion_Regulation_ClientType 
Set GameVersion_Regulation_ClientType.ProductionUploadDate = 
 case when GameVersion_Regulation_ClientType.QAApprovalDate < GameVersion_Regulation_ClientType.PMApprovalDate 
							  then GameVersion_Regulation_ClientType.PMApprovalDate 
							  else GameVersion_Regulation_ClientType.QAApprovalDate
							  end
FROM            GameVersion_Regulation_ClientType INNER JOIN
                         GameVersion_Regulation ON GameVersion_Regulation_ClientType.GameVersionRegulation_ID = GameVersion_Regulation.GameVersionRegulation_ID INNER JOIN
                         GameVersion_Property ON GameVersion_Regulation.GameVersion_ID = GameVersion_Property.GameVersion_ID AND 
                         GameVersion_Regulation.Regulation = GameVersion_Property.Regulation AND 
                         GameVersion_Regulation_ClientType.ClientType_ID = GameVersion_Property.PropertySet
WHERE        (GameVersion_Property.PropertyName = 'State') AND (GameVersion_Property.PropertyValue = 'Production')
