using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Data.LinqToSql.RecordsSynchronization;

namespace GamesPortal.Service.Artifactory
{
    public class GameVersionLanguageRecordsSynchronizer : ChildRecordsSynchronizer<GameVersion, GameVersion_Language>
    {
        public GameVersionLanguageRecordsSynchronizer()
            : base(gameVersion => gameVersion.GameVersion_Languages,
                   RecordsSynchronizerFactory<GameVersion_Language>.RecordsComparer(languageRecord => new { languageRecord.Language, languageRecord.Regulation, languageRecord.LanguageHash }),
                   RecordsSynchronizerFactory<GameVersion_Language>.PropertiesProvider())
        {

        }


        protected override void UpdateProperty(GameVersion_Language oldRecord, GameVersion_Language newRecord, PropertyInfo propertyInfo)
        {
            if (ShouldUpdateProperty(oldRecord, propertyInfo))
            {
                base.UpdateProperty(oldRecord, newRecord, propertyInfo);
            }
        }

        private bool ShouldUpdateProperty(GameVersion_Language oldRecord, PropertyInfo property)
        {
            if (property.GetValue(oldRecord) == null)
                return true;

            return property.Name != nameof(GameVersion_Language.QAApprovalDate)
                    && property.Name != nameof(GameVersion_Language.QAApprovalUser)
                    && property.Name != nameof(GameVersion_Language.ProductionUploadDate);

        }
    }
}
