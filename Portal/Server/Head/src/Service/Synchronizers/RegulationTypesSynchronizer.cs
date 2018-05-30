using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Synchronizers
{
    public class RegulationTypesSynchronizer : TimerBasedSynchronizer
    {
        public RegulationTypesSynchronizer(IGamesPortalInternalServices services)
            : base(services, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
        {     
        }
        
     
        protected override void DoWork()
        {
            using (var dbSdm = Services.CreateSdmDbContext())
            using (var dbGamesPortal = Services.CreateGamesPortalDBDataContext())
            {

                var platformRegulationTypes = dbSdm.GetTable<SDM.RegulationType>()
                                                           .ToDictionary(row => row.RGLT_Description.Replace(" ", ""),
                                                                         StringComparer.OrdinalIgnoreCase);



                foreach (var regulationNameRow in dbGamesPortal.GetTable<RegulationsName>().ToArray())
                {

                    var regulationTypesTable = dbGamesPortal.GetTable<DataAccessLayer.RegulationType>();

                    if (regulationTypesTable.Any(row => row.RegulationName == regulationNameRow.Name))
                        continue;


                    if (!platformRegulationTypes.ContainsKey(regulationNameRow.Name))
                    {
                        Logger.Error(string.Format("Can't find the regulation ID for regulation {0}", regulationNameRow.Name));
                        continue;
                    }


                    regulationTypesTable.InsertOnSubmit(new RegulationType()
                    {
                        RegulationName = regulationNameRow.Name,
                        RegulationType_ID = platformRegulationTypes[regulationNameRow.Name].RGLT_ID
                    });
                }

                dbGamesPortal.SubmitChanges();

                Services.RegulationsDictionary.Refresh();
            }
        }
    }
    
}
