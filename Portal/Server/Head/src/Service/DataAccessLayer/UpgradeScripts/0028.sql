Update [dbo].[GameVersion_Regulation_ClientType] Set [QAApprovalDate] = Approved.ApprovalDate, [QAApprovalUser] = Approved.ApprovalUser
from [GameVersion_Regulation_ClientType] inner join 
(
SELECT        GameVersionRegulationClient_ID, GameVersionRegulation_ID, ClientType_ID, PropertyName, PropertyValue, MAX(ApprovalDate) AS ApprovalDate, 
                         MAX(ApprovalUser) AS ApprovalUser
FROM            (SELECT        GameVersion_Regulation_ClientType.GameVersionRegulationClient_ID, GameVersion_Regulation_ClientType.GameVersionRegulation_ID, 
                                                    GameVersion_Regulation_ClientType.ClientType_ID, GameVersion_Property.PropertyName, GameVersion_Property.PropertyValue, 
                                                    CASE WHEN PropertyHistory.ChangeDate IS NULL THEN GameVersion_Property.LastChange ELSE PropertyHistory.ChangeDate END AS ApprovalDate, 
                                                    CASE WHEN PropertyHistory.ChangedBy IS NULL 
                                                    THEN GameVersion_Property.ChangedBy ELSE PropertyHistory.ChangedBy END AS ApprovalUser
                          FROM            GameVersion_Regulation_ClientType INNER JOIN
                                                    GameVersion_Regulation ON 
                                                    GameVersion_Regulation_ClientType.GameVersionRegulation_ID = GameVersion_Regulation.GameVersionRegulation_ID INNER JOIN
                                                    ClientType ON GameVersion_Regulation_ClientType.ClientType_ID = ClientType.ClientType_ID INNER JOIN
                                                    GameVersion_Property ON ClientType.ClientType_ID = GameVersion_Property.PropertySet AND 
                                                    GameVersion_Regulation.GameVersion_ID = GameVersion_Property.GameVersion_ID AND 
                                                    GameVersion_Regulation.Regulation = GameVersion_Property.Regulation LEFT OUTER JOIN
                                                        (SELECT        GameVersion_ID, Regulation, PropertyKey, ChangeDate, ChangedBy
                                                          FROM            GameVersion_Property_History
                                                          WHERE        (PropertyKey LIKE '%.State') AND (NewValue IN ('Approved', 'Production', 'Certificate'))) AS PropertyHistory ON 
                                                    GameVersion_Property.Regulation = PropertyHistory.Regulation AND GameVersion_Property.GameVersion_ID = PropertyHistory.GameVersion_ID AND 
                                                    GameVersion_Property.PropertyKey = PropertyHistory.PropertyKey
                          WHERE        (GameVersion_Property.PropertyName = N'State') AND (GameVersion_Property.PropertyValue IN ('Approved', 'Production', 'Certificate'))) AS LatestApproved
GROUP BY GameVersionRegulationClient_ID, GameVersionRegulation_ID, ClientType_ID, PropertyName, PropertyValue
) as Approved

on [GameVersion_Regulation_ClientType].GameVersionRegulationClient_ID = Approved.GameVersionRegulationClient_ID

GO

Update [dbo].[GameVersion_Regulation_ClientType] Set PMApprovalDate = Approved.ApprovalDate, PMApprovalUser = Approved.ApprovalUser
from [GameVersion_Regulation_ClientType] inner join 
(
SELECT        GameVersionRegulationClient_ID, GameVersionRegulation_ID, ClientType_ID, PropertyName, PropertyValue, MAX(ApprovalDate) AS ApprovalDate, 
                         MAX(ApprovalUser) AS ApprovalUser
FROM            (SELECT        GameVersion_Regulation_ClientType.GameVersionRegulationClient_ID, GameVersion_Regulation_ClientType.GameVersionRegulation_ID, 
                                                    GameVersion_Regulation_ClientType.ClientType_ID, GameVersion_Property.PropertyName, GameVersion_Property.PropertyValue, 
                                                    CASE WHEN PropertyHistory.ChangeDate IS NULL THEN GameVersion_Property.LastChange ELSE PropertyHistory.ChangeDate END AS ApprovalDate, 
                                                    CASE WHEN PropertyHistory.ChangedBy IS NULL 
                                                    THEN GameVersion_Property.ChangedBy ELSE PropertyHistory.ChangedBy END AS ApprovalUser
                          FROM            GameVersion_Regulation_ClientType INNER JOIN
                                                    GameVersion_Regulation ON 
                                                    GameVersion_Regulation_ClientType.GameVersionRegulation_ID = GameVersion_Regulation.GameVersionRegulation_ID INNER JOIN
                                                    ClientType ON GameVersion_Regulation_ClientType.ClientType_ID = ClientType.ClientType_ID INNER JOIN
                                                    GameVersion_Property ON ClientType.ClientType_ID = GameVersion_Property.PropertySet AND 
                                                    GameVersion_Regulation.GameVersion_ID = GameVersion_Property.GameVersion_ID AND 
                                                    GameVersion_Regulation.Regulation = GameVersion_Property.Regulation LEFT OUTER JOIN
                                                        (SELECT        GameVersion_ID, Regulation, PropertyKey, ChangeDate, ChangedBy
                                                          FROM            GameVersion_Property_History
                                                          WHERE        (PropertyKey LIKE '%.PMApproved') AND (NewValue = 'true')) AS PropertyHistory ON 
                                                    GameVersion_Property.Regulation = PropertyHistory.Regulation AND GameVersion_Property.GameVersion_ID = PropertyHistory.GameVersion_ID AND 
                                                    GameVersion_Property.PropertyKey = PropertyHistory.PropertyKey
                          WHERE        (GameVersion_Property.PropertyName = N'PMApproved') AND (GameVersion_Property.PropertyValue = 'true')) AS LatestApproved
GROUP BY GameVersionRegulationClient_ID, GameVersionRegulation_ID, ClientType_ID, PropertyName, PropertyValue
) as Approved

on [GameVersion_Regulation_ClientType].GameVersionRegulationClient_ID = Approved.GameVersionRegulationClient_ID

GO
