using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Data.LinqToSql.RecordsSynchronization;

namespace GamesPortal.Service.Artifactory
{
    public class GameVersionRegulationRecordsSynchronizer : ChildRecordsSynchronizer<GameVersion, GameVersion_Regulation>
    {
        public GameVersionRegulationRecordsSynchronizer()
            : base(gameVersion => gameVersion.GameVersion_Regulations,
                  RecordsSynchronizerFactory<GameVersion_Regulation>.RecordsComparer(regulationRecord => regulationRecord.Regulation),
                  RecordsSynchronizerFactory<GameVersion_Regulation>.PropertiesProvider())
        {
        }

        protected override void UpdateProperty(GameVersion_Regulation oldRecord, GameVersion_Regulation newRecord, PropertyInfo propertyInfo)
        {
            if (ShouldUpdateProperty(oldRecord, newRecord, propertyInfo))
            {
                base.UpdateProperty(oldRecord, newRecord, propertyInfo);
            }
        }

        private bool ShouldUpdateProperty(GameVersion_Regulation oldRecord,
                                        GameVersion_Regulation newRecord,
                                        PropertyInfo property)
        {


            if (property.Name != nameof(GameVersion_Regulation.QAApprovalDate)
                    && property.Name != nameof(GameVersion_Regulation.QAApprovalUser)
                    && property.Name != nameof(GameVersion_Regulation.PMApprovalDate)
                    && property.Name != nameof(GameVersion_Regulation.PMApprovalUser)
                    && property.Name != nameof(GameVersion_Regulation.ProductionUploadDate))
            {
                return true;
            }

//#warning maybe I should update property also when property.GetValue(newRecord) == null
            if (property.GetValue(oldRecord) == null || property.GetValue(newRecord) == null)
                return true;

            return false;
        }
    }
}
