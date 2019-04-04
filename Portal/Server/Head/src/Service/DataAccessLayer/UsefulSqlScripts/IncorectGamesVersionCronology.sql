
SELECT        CASE WHEN V1.Technology = 0 THEN 'Flash' ELSE 'HTML5' END AS GameTechnology, Game.MainGameType, Game.GameName, V1.Version AS Version_1, 
                         V1.CreatedDate AS CreatedDate_1, V1.CreatedBy AS CreatedBy_1, V2.Version AS Version_2, V2.CreatedDate AS CreatedDate_2, V2.CreatedBy AS CreatedBy_2, 
                         DATEDIFF(day, V1.CreatedDate, V2.CreatedDate) AS NumberOfDaysBetweenVersions
FROM            GameVersion AS V1 INNER JOIN
                         GameVersion AS V2 ON V1.Game_ID = V2.Game_ID AND V1.Technology = V2.Technology AND V1.CreatedDate < V2.CreatedDate AND 
                         V1.VersionAsLong > V2.VersionAsLong AND V1.GameVersion_ID <> V2.GameVersion_ID INNER JOIN
                         Game ON V1.Game_ID = Game.Game_ID
ORDER BY V1.Technology, Game.MainGameType, Game.GameName, V1.VersionAsLong


SELECT        CASE WHEN V1.Technology = 0 THEN 'Flash' ELSE 'HTML5' END AS GameTechnology, Game.MainGameType, Game.GameName, V1.Version AS Version_1, VP1.VersionPropertyValue as VersionProperty_1,
                         V1.CreatedDate AS CreatedDate_1, V1.CreatedBy AS CreatedBy_1, V2.Version AS Version_2, VP2.VersionPropertyValue  as VersionProperty_2, V2.CreatedDate AS CreatedDate_2, V2.CreatedBy AS CreatedBy_2, 
                         DATEDIFF(day, V1.CreatedDate, V2.CreatedDate) AS NumberOfDaysBetweenVersions
FROM            GameVersion AS V1 INNER JOIN
                         GameVersion AS V2 ON V1.Game_ID = V2.Game_ID AND V1.Technology = V2.Technology AND V1.CreatedDate < V2.CreatedDate AND 
                         V1.VersionAsLong > V2.VersionAsLong AND V1.GameVersion_ID <> V2.GameVersion_ID INNER JOIN
                         Game ON V1.Game_ID = Game.Game_ID
						 left outer join (SELECT GameVersion_ID, Max(PropertyValue) as VersionPropertyValue FROM GameVersion_Property WHERE PropertyKey = 'version' group by GameVersion_ID) VP1 
						 on V1.GameVersion_ID = VP1.GameVersion_ID
						 left outer join (SELECT GameVersion_ID, Max(PropertyValue) as VersionPropertyValue FROM GameVersion_Property WHERE PropertyKey = 'version' group by GameVersion_ID) VP2
						 on V2.GameVersion_ID = VP2.GameVersion_ID
ORDER BY V1.Technology, Game.MainGameType, Game.GameName, V1.VersionAsLong


select * from (
SELECT        CASE WHEN V1.Technology = 0 THEN 'Flash' ELSE 'HTML5' END AS GameTechnology, Game.MainGameType, Game.GameName, V1.Version AS Version_1, VP1.VersionPropertyValue as VersionProperty_1,
                         V1.CreatedDate AS CreatedDate_1, V1.CreatedBy AS CreatedBy_1, V2.Version AS Version_2, VP2.VersionPropertyValue  as VersionProperty_2, V2.CreatedDate AS CreatedDate_2, V2.CreatedBy AS CreatedBy_2, 
                         DATEDIFF(day, V1.CreatedDate, V2.CreatedDate) AS NumberOfDaysBetweenVersions, V1.VersionAsLong as V1AsLong
FROM            GameVersion AS V1 INNER JOIN
                         GameVersion AS V2 ON V1.Game_ID = V2.Game_ID AND V1.Technology = V2.Technology AND V1.CreatedDate < V2.CreatedDate AND 
                         V1.VersionAsLong > V2.VersionAsLong AND V1.GameVersion_ID <> V2.GameVersion_ID INNER JOIN
                         Game ON V1.Game_ID = Game.Game_ID
						 left outer join (SELECT GameVersion_ID, Max(PropertyValue) as VersionPropertyValue FROM GameVersion_Property WHERE PropertyKey = 'version' group by GameVersion_ID) VP1 
						 on V1.GameVersion_ID = VP1.GameVersion_ID
						 left outer join (SELECT GameVersion_ID, Max(PropertyValue) as VersionPropertyValue FROM GameVersion_Property WHERE PropertyKey = 'version' group by GameVersion_ID) VP2
						 on V2.GameVersion_ID = VP2.GameVersion_ID
) z where Version_1 <> VersionProperty_1 or Version_2 != VersionProperty_2

ORDER BY GameTechnology, MainGameType, GameName, V1AsLong



SELECT GameVersion_ID, Max(PropertyValue) as VersionPropertyValue FROM GameVersion_Property WHERE PropertyKey = 'version' group by GameVersion_ID



--delete from GameVersion where GameVersion_ID not in (Select GameVersion_ID From GameVersion_Property)
							   




SELECT        GameTechnology.TechnologyName, Game.MainGameType, Game.GameName, GP.Regulation, GameVersion.Version
FROM            (SELECT        GameVersion_ID, Regulation
                          FROM            GameVersion_Property
                          GROUP BY GameVersion_ID, Regulation) AS GP INNER JOIN
                         GameVersion ON GP.GameVersion_ID = GameVersion.GameVersion_ID INNER JOIN
                         Game ON GameVersion.Game_ID = Game.Game_ID INNER JOIN
                         GameTechnology ON GameVersion.Technology = GameTechnology.Technology_ID LEFT OUTER JOIN
                             (SELECT        GameVersion_ID, Regulation, PropertyKey
                               FROM            GameVersion_Property AS GameVersion_Property_1
                               WHERE        (PropertyKey = 'version')) AS V ON GP.GameVersion_ID = V.GameVersion_ID AND GP.Regulation = V.Regulation
WHERE        (V.PropertyKey IS NULL)
order by TechnologyName, MainGameType, GameName, Regulation, GameVersion.VersionAsLong