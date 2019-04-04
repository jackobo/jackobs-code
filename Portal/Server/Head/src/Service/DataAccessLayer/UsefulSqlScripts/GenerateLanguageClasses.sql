select 'public static readonly Language ' 
	 + Replace(Replace(LNG_FullName, ' ', ''), '-', '_')
	 + ' = '
	 + 'new Language('
	 + '"' + CONVERT(nvarchar(5), LNG_ID) + '", '
	 + '"' + LNG_FullName + '", '
	 + '"' + LNG_ISO_ALPHA2 + '", '
	 + '"' + LNG_ISO_ALPHA3_1 + '", '
	 + case when LNG_ISO_ALPHA3_2 is null then 'null' else '"' + LNG_ISO_ALPHA3_2 + '"' end 
	 +');'	
from [MD].[_Languages]  where  [LNG_ISO_ALPHA3_1] is not null